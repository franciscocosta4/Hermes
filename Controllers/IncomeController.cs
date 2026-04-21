using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hermes.Models;
using Microsoft.AspNetCore.Identity;

namespace Hermes.Controllers;

[Authorize]
public class IncomeController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IncomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);

        var model = new IncomeViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Initial = user.FullName?[0].ToString().ToUpper()
        };

        return View("~/Views/Dashboard/create-income.cshtml", model);
    }

}