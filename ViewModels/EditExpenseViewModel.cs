using System.ComponentModel.DataAnnotations;
using Hermes.Models;

namespace Hermes.Models
{
    public class EditExpenseViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }

        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue,  ErrorMessage = "The amount has to be greater than 0" )]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        // aqui vai o id da categoria escolhida pelo user:  <option value="@category.Id"> 
        [Required]
        public int CategoryId { get; set; }
    }
}