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

        var model = new IncomeViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
            Date = DateOnly.FromDateTime(DateTime.Today)
        };

        return View("~/Views/Dashboard/create-income.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeViewModel model)
    {
        // verificamos se os dados do input do CreateIncomeViewModel são válidos
        if (!ModelState.IsValid)
            return View(model);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        //criamos um income que usa o model "Income" pois o "CreateIncomeViewModel" não pode ter acesso 
        //aos dados do user e nos precisamos de registar o user assossiado ao income
        var income = new Income
        {
            Amount = model.Amount,
            Date = model.Date,
            UserId = user.Id
        };

        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }
}