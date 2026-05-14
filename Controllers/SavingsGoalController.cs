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

        var Goal = new SavingsGoal
        {
            Target_amount = goal.Target_amount,
            Name= goal.Name,
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