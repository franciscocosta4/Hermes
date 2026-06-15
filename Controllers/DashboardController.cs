using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;
using Npgsql;

namespace Hermes.Controllers;

[Authorize]
public class DashboardController : Controller
{

    private readonly string _connectionString;

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(IConfiguration configuration, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
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
        //! EXISTEM 2 TIPOS DE OVERSPENDING: GASTAR MAIS QUE A MEDIA DOS 90 DIAS (isOverspending) OU GASTAR MAIS Q O BUDGET (isBudgetOverspent)
        // se MonthExpenseSum for maior que Sum90DaysExpenses fica true
        bool isOverspending = MonthExpenseSum > averageMonthly90Days;
        bool isBudgetOverspent = false;

        int mesAtual = DateTime.Now.Month;
        var currentBudget = _context.Budgets.Where(e => e.UserId == userid && e.Month == mesAtual).Select(e => new
        {
            e.Limit_amount
        }).FirstOrDefault();
        decimal currentBudgetLimit = 0;
        decimal diffBudgetToExpense = 0;
        decimal budgetUsedPercentage = 0;

        if (currentBudget != null)
        {
            currentBudgetLimit = currentBudget.Limit_amount;

            // calcula a diferença entre o budget e as desepesas do mes 
            diffBudgetToExpense = currentBudgetLimit - MonthExpenseSum;

            // se limite exisir: 
            if (currentBudgetLimit > 0)
            {
                // calcula a percentagem de budget que já foi usado
                budgetUsedPercentage =
                    Math.Min(
                        (MonthExpenseSum / currentBudgetLimit) * 100,
                        100
                    );
            }

            if (MonthExpenseSum > currentBudgetLimit)
            {
                isBudgetOverspent = true;
            }
        }
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
            CurrentBudgetLimit = currentBudgetLimit,
            DiffBudgetToExpense = diffBudgetToExpense,
            BudgetUsedPercentage = budgetUsedPercentage,
            isBudgetOverspent = isBudgetOverspent,
            Overspending = isOverspending
        };

        return View(model);

    }
    public async Task<IActionResult> GenerateCsv()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var userId = _userManager.GetUserId(User);

        Response.ContentType = "text/csv";
        Response.Headers.Append("Content-Disposition", "attachment; filename=teste.csv");

        using var exporter = await connection.BeginTextExportAsync(
            $"COPY (" +
            $"SELECT 'EXPENSE' AS \"Type\", \"Description\", \"Amount\", \"Date\" FROM \"Expenses\" WHERE \"UserId\" = '{userId}' " +
            $"UNION ALL " +
            $"SELECT 'INCOME' AS \"Type\", NULL AS \"Description\", \"Amount\", \"Date\" FROM \"Incomes\" WHERE \"UserId\" = '{userId}' " +
            $") TO STDOUT WITH (FORMAT CSV, HEADER TRUE)"
        );

        char[] buffer = new char[8192];
        int read;

        while ((read = await exporter.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(buffer, 0, read);
            await Response.Body.WriteAsync(bytes);
        }

        return new EmptyResult();
    }

}