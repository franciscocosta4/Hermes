using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
public class CreateIncomeViewModel
{

    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Initial { get; set; }


    [Required(ErrorMessage = "The value is required")]
    [Range(0.01, 999999999, ErrorMessage = "The amount has to be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }
}
}