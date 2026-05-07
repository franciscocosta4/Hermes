using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class BudgetController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public BudgetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userid = _userManager.GetUserId(User);

        var last30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

        // percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthExpenseSum = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        // pegamos em todos os budgets do user para mostrar na table
        var AllBudgets = _context.Budgets
            .Where(e => e.UserId == userid)
            .Select(b => new Budget
            {
                Limit_amount = b.Limit_amount,
                Month = b.Month,
                Year = b.Year
            })
            .ToList();

        int mesAtual = DateTime.Now.Month;
        decimal CurrentBudget = _context.Budgets.Where(e => e.UserId == userid && e.Month == mesAtual).Select(e => e.Limit_amount).FirstOrDefault(); ;
        decimal DiffBudgetToExpense = CurrentBudget - MonthExpenseSum;

        // Evita divisão por zero
        decimal BudgetUsedPercentage = 0;

        if (CurrentBudget > 0)
            BudgetUsedPercentage = MonthExpenseSum / CurrentBudget * 100;



        var model = new BudgetViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            AllBudgets = AllBudgets,
            MonthExpenseSum = MonthExpenseSum,
            CurrentBudget = CurrentBudget,
            DiffBudgetToExpense = DiffBudgetToExpense,
            BudgetUsedPercentage = BudgetUsedPercentage,
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new CreateBudgetViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBudgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);

        }

        var user = await _userManager.GetUserAsync(User);
        var userid = _userManager.GetUserId(User);

        var exists = _context.Budgets.Any(b => // procura por um budget naquele mês para o user logado
            b.UserId == userid &&
            b.Month == model.Month &&
            b.Year == model.Year);

        if (exists) // verifica se já existe um budget naquele mês para o user logado
        {
            ModelState.AddModelError("", "There is already a budget for this month.");
            return View(model);
        }

        var Budget = new Budget
        {
            Limit_amount = model.Limit_amount,
            Month = model.Month,
            Year = model.Year,
            UserId = user.Id
        };

        _context.Budgets.Add(Budget);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Budget");
    }

    public IActionResult Delete(int id)
    {
        var Budget = _context.Budgets.Find(id);

        if (Budget == null)
        {
            return NotFound();
        }
        _context.Budgets.Remove(Budget);
        _context.SaveChanges();

        return RedirectToAction("Index", "Dashboard");
    }
}