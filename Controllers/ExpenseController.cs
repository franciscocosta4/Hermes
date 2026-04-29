using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;
namespace Hermes.Controllers;

[Authorize]
public class ExpenseController : Controller
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
    public ExpenseController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        // não precisa de ser assincrona pois nao vai a bd, apenas pega no id do User
        var userid = _userManager.GetUserId(User);
        // aqui percorre as categorias e guarda as relacionadas ao user logado
        var categories = _context.Categories.Where(c => c.UserId == userid).ToList();

        // Neste caso usamos ViewBag por ser mais simples de preencher e enviar dados para a view,
        // especialmente para listas pequenas num dropdown.
        // Evita termos de passar mais 2 parametros na ExpenseViewModel
        ViewBag.Categories = categories;

        var model = new CreateExpenseViewModel
        {
            // estes dados são passados pois são precisos para a sidebar e formatação da data
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(), 
            Date = DateOnly.FromDateTime(DateTime.Today)
        };

        return View("create", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseViewModel model)
    {
        // verificamos se os dados do input do CreateExpenseViewModel são válidos
        if (!ModelState.IsValid)
            return View(model);

        // pega no user autenticado
        // usamos await porque a query pode demorar um pouco 
        var user = await _userManager.GetUserAsync(User); // retorna: Task<User> 

        // Criamos uma Expense porque é a entidade que representa a tabela na base de dados.
        // O CreateExpenseViewModel só serve para receber dados da view e não contém informação de domínio (como UserId).
        var expense = new Expense
        {
            Amount = model.Amount,
            Description = model.Description,
            Date = model.Date,
            UserId = user.Id,
            CategoryId = model.CategoryId // aqui tá o id que vem da view, passa pelo CreateExpenseViewModel e chega aqui
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id){
        var user = await _userManager.GetUserAsync(User);
        // não precisa de ser assincrona pois nao vai a bd, apenas pega no id do User
        var userid = _userManager.GetUserId(User);
        // aqui percorre as categorias e guarda as relacionadas ao user logado
        var categories = _context.Categories.Where(c => c.UserId == userid).ToList();
        // ainda precisamos de preencher a viewbag para o select com as categorias
        ViewBag.Categories = categories;

        var expense = _context.Expenses.Find(id);
        if (expense == null)
        {
            return NotFound();
        }

        var model = new EditExpenseViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Amount = expense.Amount,
            Description = expense.Description,
            Date = expense.Date,
            // UserId = user.Id,
            CategoryId = expense.CategoryId 
        };
        return View("edit", model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditExpenseViewModel editedExpense)
    {
        // verificamos se os dados do input são válidos
        if (!ModelState.IsValid)
            return View(editedExpense);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        var existingExpense = _context.Expenses.Find(editedExpense.Id);
        if (existingExpense.UserId != user.Id)
            return Forbid();

        existingExpense.Amount = editedExpense.Amount;
        existingExpense.Description = editedExpense.Description;
        existingExpense.Date = editedExpense.Date;
        existingExpense.CategoryId = editedExpense.CategoryId;
        // existingExpense.UserId = user.Id;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }


    [HttpPost]
    [ValidateAntiForgeryToken] // Este atributo valida o token anti-CSRF (Cross-Site Request Forgery)
                               // Garante que o pedido vem realmente do nosso site e não de um site malicioso
    public IActionResult Delete(int id)
    {
        var expense = _context.Expenses.Find(id);

        if (expense== null)
        {
            return NotFound();
        }

        // Remove o livro do contexto da base de dados
        _context.Expenses.Remove(expense);
        // Guarda as alterações na base de dados (executa o comando SQL de eliminação)
        _context.SaveChanges();

        return RedirectToAction("Index", "Dashboard");
    }

}