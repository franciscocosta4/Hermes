using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;

namespace Hermes.Controllers;

[Authorize]
public class BudgetController : Controller
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
    public BudgetController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new CreateBudgetViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper(),
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBudgetViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        var Budget = new Budget
        {
            Limit_amount = model.Limit_amount,
            Month = model.Month,
            Year = model.Year,
            UserId = user.Id
        };

        _context.Budgets.Add(Budget);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Delete(int id)
    {
        var Budget = _context.Budgets.Find(id);

        if (Budget == null)
        {
            return NotFound();
        }
        _context.Budgets.Remove(Budget);
        _context.SaveChanges();

        return RedirectToAction("Index", "Dashboard");
    }
}