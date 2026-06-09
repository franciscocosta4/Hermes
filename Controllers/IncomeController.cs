using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class IncomeController : Controller
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
    public IncomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new CreateIncomeViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Date = DateOnly.FromDateTime(DateTime.Today)
        };

        return View("create", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeViewModel model)
    {
        // verificamos se os dados do input do CreateIncomeViewModel são válidos
        if (!ModelState.IsValid)
            return View(model);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        // Criamos um Income porque é a entidade que representa a tabela incomes na base de dados.
        // O CreateIncomeViewModel só serve para receber dados da view e não contém informação de domínio (como UserId).
        var income = new Income
        {
            Amount = model.Amount,
            Date = model.Date,
            UserId = user.Id
        };

        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        //* pega em todos os goals do user
        var goals = _context.SavingsGoals.Where(g => g.UserId == user.Id && g.percentage_of_income > 0).ToList();

        foreach (var goal in goals)
        {
            decimal percentage = goal.percentage_of_income ?? 0m;

            decimal amountToSave = income.Amount * percentage / 100m; //* calcula a quantidade para mandar para o goal

            if (amountToSave <= 0)
                continue;

            var expense = new Expense //* cria automaticamente uma "despesa" para que o dinheiro possa ser mandado para o goal
            {
                Description = $"Automatic allocation to {goal.Name}",
                Amount = amountToSave,
                Date = income.Date,
                UserId = user.Id,
                CategoryId = 1 // categoria Savings criada por seed
            };

            _context.Expenses.Add(expense);

            goal.Current_amount += amountToSave; //* atualiza a quantidade guardada no goal
        }
        
        
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var income = _context.Incomes.Find(id);

        if (income == null)
        {
            return NotFound();
        }

        var model = new EditIncomeViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Amount = income.Amount,
            Date = income.Date,
        };
        return View("edit", model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditIncomeViewModel editedIncome)
    {
        // verificamos se os dados do input do CreateIncomeViewModel são válidos
        if (!ModelState.IsValid)
            return View(editedIncome);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        var existingIncome = _context.Incomes.Find(editedIncome.Id);
        if (existingIncome.UserId != user.Id)
            return Forbid();

        existingIncome.Amount = editedIncome.Amount;
        existingIncome.Date = editedIncome.Date;
        // existingIncome.UserId = user.Id;

        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Este atributo valida o token anti-CSRF (Cross-Site Request Forgery)
                               // Garante que o pedido vem realmente do nosso site e não de um site malicioso
    public IActionResult Delete(int id)
    {
        var income = _context.Incomes.Find(id);

        if (income == null)
        {
            return NotFound();
        }
        _context.Incomes.Remove(income);
        _context.SaveChanges();

        return RedirectToAction("Index", "Dashboard");
    }
}