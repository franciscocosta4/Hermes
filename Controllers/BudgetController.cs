using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
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
        if (user is null)
        {
            return Challenge();
        }

        var userid = _userManager.GetUserId(User);

        var last30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

        // percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthExpenseSum = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        // pegamos em todos os budgets do user para mostrar na table
        var AllBudgets = _context.Budgets
            .Where(e => e.UserId == userid)
            .Select(b => new Budget
            {
                Id = b.Id,
                Limit_amount = b.Limit_amount,
                Month = b.Month,
                Year = b.Year
            })
            .ToList();

        int mesAtual = DateTime.Now.Month;
        var CurrentBudget = _context.Budgets
    .Where(e => e.UserId == userid && e.Month == mesAtual)
    .Select(e => new
    {
        e.Id,
        e.Limit_amount
    }).FirstOrDefault();

        decimal currentBudgetLimit = 0;
        int currentBudgetId = 0;
        decimal diffBudgetToExpense = 0;
        int budgetId = 0;
        decimal budgetUsedPercentage = 0;


        if (CurrentBudget != null)
        {
            currentBudgetLimit = CurrentBudget.Limit_amount;
            currentBudgetId = CurrentBudget.Id;
            budgetId = CurrentBudget.Id;

            diffBudgetToExpense = currentBudgetLimit - MonthExpenseSum;

            budgetUsedPercentage = 0;

            // Evita divisão por zero
            if (currentBudgetLimit > 0)
            {
                budgetUsedPercentage = MonthExpenseSum / currentBudgetLimit * 100;
            }
        }

        // se MonthExpenseSum for maior que budget do mes fica true
        bool isOverspending = MonthExpenseSum > currentBudgetLimit;

        var model = new BudgetViewModel
        {
            FullName = user.FullName ?? "User",
            Email = user.Email ?? string.Empty,
            Initial = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName!.Trim()[..1].ToUpper() : "U",
            AllBudgets = AllBudgets,
            MonthExpenseSum = MonthExpenseSum,
            CurrentBudgetLimit = currentBudgetLimit,
            CurrentBudgetId = currentBudgetId,
            DiffBudgetToExpense = diffBudgetToExpense,
            BudgetUsedPercentage = budgetUsedPercentage,
            isOverspending = isOverspending,
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        var model = new CreateBudgetViewModel
        {
            FullName = user.FullName ?? "User",
            Email = user.Email ?? string.Empty,
            Initial = !string.IsNullOrWhiteSpace(user.FullName) ? user.FullName!.Trim()[..1].ToUpper() : "U",
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Create(CreateBudgetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);

        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

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

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("auth")]
    public IActionResult Delete(int id)
    {
        // Obtém o ID do utilizador autenticado.
        var userId = _userManager.GetUserId(User);

        var Budget = _context.Budgets.Find(id);

        if (Budget == null)
        {
            return NotFound();
        }

        if (Budget.UserId != userId)
        {
            return Forbid();
        }
        _context.Budgets.Remove(Budget);
        _context.SaveChanges();

        return RedirectToAction("Index", "Budget");
    }
}