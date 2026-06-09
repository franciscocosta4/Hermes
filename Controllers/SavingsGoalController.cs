using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class SavingsGoalController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public SavingsGoalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = _userManager.GetUserId(User);

        var allGoals = await _context.SavingsGoals
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.Id)
            .ToListAsync();

        var vm = new SavingsGoalViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            AllGoals = allGoals
        };
        return View(vm);
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new CreateSavingsGoalViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Current_amount = 0,// o goal inicia como 0 por padrão
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSavingsGoalViewModel goal)
    {

        var user = await _userManager.GetUserAsync(User);
        var userid = _userManager.GetUserId(User);

        // Contar quantos goals já tem
        var totalGoals = await _context.SavingsGoals
            .CountAsync(g => g.UserId == userid);

        // Limite de 3
        if (totalGoals >= 3)
        {
            ModelState.AddModelError("", "Só podes ter até 3.");
            return View(goal);
        }


        // Datas limite (30 e 90 dias)
        var last30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var last90Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-90));

        // percorre os incomes e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthIncomeSum = _context.Incomes.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        // percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthExpenseSum = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        decimal MonthBalance = MonthIncomeSum - MonthExpenseSum;

        if (goal.minimum_balance_to_keep > 0)
        {
            decimal minimum = goal.minimum_balance_to_keep ?? 0m;

            decimal amountToKeep = MonthBalance - minimum;

            if (amountToKeep > 0)
            {
                var expense = new Expense
                {
                    Description = $"Transferência para objetivo {goal.Name}",
                    Amount = amountToKeep,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    UserId = user.Id,
                    CategoryId = 1
                };

                goal.Current_amount += amountToKeep; // atualiza o dinheiro poupado com o dinheiro que foi tirado do balance

                _context.Expenses.Add(expense);
            }
        }

        //* procura e soma todas as percentagens que o user definiu para saber se passa de 100%
        var totalPercentage = await _context.SavingsGoals.Where(g => g.UserId == userid).SumAsync(g => g.percentage_of_income ?? 0m);

        if (totalPercentage + (goal.percentage_of_income ?? 0m) > 100)
        {
            ModelState.AddModelError(
                "percentage_of_income",
                "The total percentage allocated across all goals cannot exceed 100%."
            );

            return View(goal);
        }

        var Goal = new SavingsGoal
        {
            Target_amount = goal.Target_amount,
            Name = goal.Name,
            Current_amount = goal.Current_amount,
            percentage_of_income = goal.percentage_of_income,
            minimum_balance_to_keep = goal.minimum_balance_to_keep,
            UserId = user.Id
        };

        _context.SavingsGoals.Add(Goal);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    public IActionResult Delete(int id)
    {
        var Goal = _context.SavingsGoals.Find(id);

        if (Goal == null)
        {
            return NotFound();
        }
        _context.SavingsGoals.Remove(Goal);
        _context.SaveChanges();

        return RedirectToAction("Index", "SavingsGoal");
    }
}