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

        var primaryGoal = allGoals.FirstOrDefault();

        var vm = new SavingsGoalViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            CurrentGoalId = primaryGoal?.Id,
            CurrentGoalName = primaryGoal?.Name ?? "No active goal",
            CurrentTargetAmount = primaryGoal?.Target_amount ?? 0,
            CurrentSavedAmount = primaryGoal?.Current_amount ?? 0,
            CurrentPercentageOfIncome = primaryGoal?.percentage_of_income,
            CurrentMinimumBalanceToKeep = primaryGoal?.minimum_balance_to_keep,
            AllGoals = allGoals
        };

        if (primaryGoal != null && primaryGoal.Target_amount > 0)
        {
            vm.GoalProgressPercentage = Math.Round(
                (primaryGoal.Current_amount / primaryGoal.Target_amount) * 100, 1);
            vm.RemainingAmount = primaryGoal.Target_amount - primaryGoal.Current_amount;
            vm.IsCompleted = primaryGoal.Current_amount >= primaryGoal.Target_amount;
            vm.IsCloseToCompletion = vm.GoalProgressPercentage >= 90 && vm.GoalProgressPercentage < 100;
        }

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
}