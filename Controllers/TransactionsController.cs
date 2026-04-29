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
    // page vai representar a pagina atual (1 por padrão)
    // "pageSize" representa quantos registos queremos mostrar por página
    // Estes valores vêm da URL, por exemplo:
    // /Transactions/Index?page=2&pageSize=10
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
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

        // Junta incomes e expenses numa única query
        // Ainda NÃO executa na base de dados (IQueryable)
        var query = Incomes.Union(Expenses)
            .OrderByDescending(t => t.Date); // ordena por data (mais recentes primeiro)

        // Conta o total de registos existentes na query
        // Isto serve para calcular o número total de páginas
        var totalItems = query.Count();

        // Aqui aplica-se a paginação:
        // Skip = ignora os registos das páginas anteriores
        // Take = traz apenas os registos da página atual
        var transactions = query
            .Skip((page - 1) * pageSize) // calcula quantos registos saltar
            .Take(pageSize)              // limita ao tamanho da página
            .ToList();                   // executa a query na base de dados
            
        var model = new TransactionsViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            // apenas os registos da página atual
            Transactions = transactions,
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };

        return View(model);
    }
}