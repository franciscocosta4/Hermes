using System.ComponentModel.DataAnnotations;
using Hermes.Models;

// usamos um viewmodel específico para o post para evitar de o create mandar os dados da IncomeViewModel, que têm Email, fullname ...
// e ia acabar por falhar no post : [{"Field":"Email","Errors":["The Email field is required."]},{"Field":"Initial","Errors":["The Initial field is required."]},{"Field":"FullName","Errors":["The FullName field is required."]}]
public class CreateExpenseViewModel
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateOnly Date { get; set; }
    [Required]
    public Category Category { get; set; }
}