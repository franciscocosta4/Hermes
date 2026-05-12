using Microsoft.EntityFrameworkCore;

namespace Hermes.Models
{
    public class SavingsGoal
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public decimal Target_amount {get; set;}
        public decimal Current_amount {get; set;}

        // define qual percentagem de cada income vai para o goal (null nao vai nada)
        public int? percentage_of_income {get; set;}

        // define qual o dinheiro que vai ficar sempre no balance (null nao vai nada)
        public decimal? minimum_balance_to_keep {get; set;}
        
        // Foreign key para o utilizador (AspNetUsers)
        public string UserId { get; set; }
        // Propriedade de navegação (many-to-one)
        public ApplicationUser User { get; set; }


    }
}
