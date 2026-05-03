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
        // Datas limite (30 e 90 dias)
        var last30Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
        var last90Days = DateOnly.FromDateTime(DateTime.Now.AddDays(-90));

        // percorre os incomes e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthIncomeSum = _context.Incomes.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        // percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 30 dias
        decimal MonthExpenseSum = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last30Days).Sum(e => e.Amount);

        // percorre as despesas e guarda as relacionadas ao user logado e registado nos últimos 90 dias
        decimal Sum90DaysExpenses = _context.Expenses.Where(e => e.UserId == userid && e.Date >= last90Days).Sum(e => e.Amount);

        // faz a media das despesas dos ultimos 3 meses e mete com 2 casas decimais
        decimal averageMonthly90Days = Math.Round(Sum90DaysExpenses / 3, 2, MidpointRounding.AwayFromZero);

        decimal MonthBalance = MonthIncomeSum - MonthExpenseSum;

        // pega nos incomes do ultimo mes e manda para DashboardTransactionViewModel
        var MonthIncomes = _context.Incomes.Where(c => c.UserId == userid && c.Date >= last30Days)
            .Select(i => new DashboardTransactionViewModel
            {
                Id = i.Id,
                Amount = i.Amount,
                Date = i.Date,
                Type = "Income",
                Category = "-"
            });

        // pega nas despesas do ultimo mes e manda para DashboardTransactionViewModel
        var MonthExpenses = _context.Expenses
            .Where(e => e.UserId == userid && e.Date >= last30Days)
            .Select(e => new DashboardTransactionViewModel
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Type = "Expense",
                Category = e.Category.Name
            });
        // junta os incomes e as despesas do mes em uma lista
        var MonthTransactions = MonthIncomes.Union(MonthExpenses).OrderBy(t => t.Date).ToList();

        // se MonthExpenseSum for maior que Sum90DaysExpenses fica true
        bool isOverspending = MonthExpenseSum > averageMonthly90Days;

        var model = new DashboardViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            MonthIncomeSum = MonthIncomeSum,
            MonthExpenseSum = MonthExpenseSum,
            MonthBalance = MonthBalance,
            averageMonthly90Days = averageMonthly90Days,
            MonthTransactions = MonthTransactions,
            Overspending = isOverspending
        };

        return View(model);


    }

}