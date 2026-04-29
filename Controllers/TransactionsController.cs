using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class TransactionsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    public TransactionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        // não precisa de ser assincrona pois nao vai a bd, apenas pega no id do User
        var userid = _userManager.GetUserId(User);

        var Incomes = _context.Incomes.Where(c => c.UserId == userid)
            .Select(i => new DashboardTransactionViewModel
            {
                Id = i.Id,
                Amount = i.Amount,
                Date = i.Date,
                Type = "Income",
                Category = "-"
            });
        var Expenses = _context.Expenses
            .Where(e => e.UserId == userid)
            .Select(e => new DashboardTransactionViewModel
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Type = "Expense",
                Category = e.Category.Name
            });
        // junta os incomes e as despesas em uma lista
        var Transactions = Incomes.Union(Expenses).OrderBy(t => t.Date).ToList();

        var model = new TransactionsViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Transactions = Transactions
        };

        return View(model);
    }
}