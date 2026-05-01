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
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchinput = null)
    {
        var user = await _userManager.GetUserAsync(User);
        var userid = _userManager.GetUserId(User);
        //percorre as categorias e guarda as relacionadas ao user logado
        var categories = _context.Categories.Where(c => c.UserId == userid).ToList();

        // Neste caso usamos ViewBag por ser mais simples de preencher e enviar dados para a view.
        // Evita termos de passar mais 2 parametros na ExpenseViewModel
        ViewBag.Categories = categories;

        var incomes = _context.Incomes
            .Where(c => c.UserId == userid) 
            .Select(i => new DashboardTransactionViewModel
            {
                Id = i.Id,
                Amount = i.Amount,
                Date = i.Date,
                Type = "Income",
                Description = "-",
                Category = "-",
            });
        var expenses = _context.Expenses
            .Where(e => e.UserId == userid)
            .Select(e => new DashboardTransactionViewModel
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Type = "Expense",
                Description = e.Description,
                Category = e.Category.Name,
            });

        // Junta primeiro em memória SÓ depois de filtrar base
        var query = incomes.Concat(expenses);

        // filtro de pesquisa
        if (!string.IsNullOrEmpty(searchinput))
        {
            searchinput = searchinput.Trim().ToLower();

            query = query.Where(t =>
                (t.Category ?? "").ToLower().Contains(searchinput) ||
                (t.Type ?? "").ToLower().Contains(searchinput) ||
                (t.Description ?? "").ToLower().Contains(searchinput)
            );

        }

        // só aqui é que ordenamos
        query = query.OrderByDescending(t => t.Date);

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
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Search = searchinput, // para manter o input do user na barra de pesquisa
        };

        return View("index", model);
    }
}