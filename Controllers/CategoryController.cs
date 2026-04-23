using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;
using Hermes.Data;
namespace Hermes.Controllers;

[Authorize]
public class CategoryController : Controller
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
    public CategoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new CategoryViewModel
        {
            // estes dados são passados pois são precisos para a sidebar
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper()
        };

        return View("~/Views/Dashboard/create-category.cshtml", model);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryViewModel model)
    {
        // verificamos se os dados do input do CreateExpenseViewModel são válidos
        if (!ModelState.IsValid)
            return View(model);

        // pega no user autenticado
        var user = await _userManager.GetUserAsync(User);

        //ao usar o model "Category" garantimos que o userId ainda pode ser registado mesmo tendo: CreateCategoryViewModel model 
        // para que o user não consiga fazer um registo com outro id, por exemplo .
        var category = new Category
        {
            Name = model.Name,
            UserId = user.Id
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }


}