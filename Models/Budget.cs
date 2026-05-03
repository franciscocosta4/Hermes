namespace Hermes.Models
{
    public class Budget
    {
        public int id {get;set;}
        public int Month {get;set;}
        public int Year {get;set;}
        public decimal Limit_amount {get;set;}

        // Foreign key para o utilizador (AspNetUsers)
        public string UserId { get; set; }
        // Propriedade de navegação (many-to-one)
        public ApplicationUser User { get; set; }
    }
    
}