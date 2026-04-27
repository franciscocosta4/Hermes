using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class ExpenseViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Initial { get; set; }
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        // como estamos a usar viewbag não precisamos de:
        
        // public int CategoryId { get; set; }  // valor do select
        // public List<Category> Categories { get; set; }
    }
}