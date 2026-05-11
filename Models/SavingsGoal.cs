using Microsoft.EntityFrameworkCore;

namespace Hermes.Models
{
    public class SavingsGoal
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public decimal Target_amount {get; set;}
        public decimal Current_amount {get; set;}
        
        // Foreign key para o utilizador (AspNetUsers)
        public string UserId { get; set; }
        // Propriedade de navegação (many-to-one)
        public ApplicationUser User { get; set; }


    }
}
