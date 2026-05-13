using System.ComponentModel.DataAnnotations;
using Hermes.Models;

namespace Hermes.Models
{
    public class CreateSavingsGoalViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }

        [Required]
        [Range(0.01, double.MaxValue,  ErrorMessage = "The amount has to be greater than 0" )]
        public decimal Target_amount {get; set;}
        public string Name {get; set;}
        public decimal Current_amount {get; set;}

        // define qual percentagem de cada income vai para o goal (null nao vai nada)
        public int? percentage_of_income {get; set;}

        // define qual o dinheiro que vai ficar sempre no balance (null nao vai nada)
        public decimal? minimum_balance_to_keep {get; set;}
    }
}