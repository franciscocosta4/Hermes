using System.ComponentModel.DataAnnotations;
using Hermes.Models;

namespace Hermes.Models
{
    public class CreateBudgetViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }

        [Required]
        [Range(0.01, double.MaxValue,  ErrorMessage = "The amount has to be greater than 0" )]
        public decimal Limit_amount {get;set;}
        // public int id {get;set;}
        public int Month {get;set;}
        public int Year {get;set;}
    }
}