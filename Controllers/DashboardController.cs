using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class DashboardController : Controller
{
    /// <summary>
    /// Campo privado que armazena a instância do contexto da base de dados.
    /// O underscore (_) antes do nome é uma convenção para indicar que é uma variável privada.
    /// Usamos 'readonly' para garantir que não pode ser alterado após inicialização.
    /// </summary>
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Construtor que injeta o contexto da base de dados.
    /// O ASP.NET Core fornece automaticamente a instância do contexto.
    /// </summary>
    /// <param name="context">Instância do contexto da base de dados</param>
    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        // não precisa de ser assincrona pois nao vai a bd, apenas pega no id do User
        var userid = _userManager.GetUserId(User);
        // Data limite em DateOnly
        var last30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var last90Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-90));

        // aqui percorre os incomes e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        var Monthincomes = _context.Incomes
            .Where(c => c.UserId == userid && c.Date >= last30Days)
            .ToList();

        decimal MonthIncomeSum = 0;
        foreach (Income income in Monthincomes)
        {
            var quantidade = income.Amount;
            MonthIncomeSum += income.Amount;
        }

        // aqui percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        var MonthExpenses = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last30Days).ToList();
        decimal MonthExpenseSum = 0; 
        foreach(Expense expense in MonthExpenses)
        {
            var quantidade = expense.Amount;
            MonthExpenseSum += expense.Amount;
        }

        var List90DaysExpenses = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last90Days).ToList();
        decimal Sum90DaysExpenses = 0; 
        foreach(Expense expense in List90DaysExpenses)
        {
            var quantidade = expense.Amount;
            Sum90DaysExpenses += expense.Amount;
        }

        var model = new DashboardViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            MonthIncomeSum = MonthIncomeSum,
            MonthExpenseSum = MonthExpenseSum,
            Sum90DaysExpenses = Sum90DaysExpenses,
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}