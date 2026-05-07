using Microsoft.EntityFrameworkCore;

namespace Hermes.Models
{
    [Index(nameof(UserId), nameof(Month), nameof(Year), IsUnique = true)] // garante que só pode haver um budget por mês para cada user
    public class Budget
    {
        public int Id {get;set;}
        public int Month {get;set;}
        public int Year {get;set;}
        public decimal Limit_amount {get;set;}

        // Foreign key para o utilizador (AspNetUsers)
        public string UserId { get; set; }
        // Propriedade de navegação (many-to-one)
        public ApplicationUser User { get; set; }
    }
    
}