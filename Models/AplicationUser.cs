using Microsoft.AspNetCore.Identity;

namespace Hermes.Models
{
    public class ApplicationUser : IdentityUser
    {
        // podemos adicionar campos extra depois (o entutycore já tras campos na tabela AspNetUsers)

        // Nome completo do utilizador
        public string FullName { get; set; }

        // Relação inversa (1 user → muitos incomes)
        public ICollection<Income> Incomes { get; set; }

        // Relação inversa (1 user → muitas categories)
        public ICollection<Category> Categories { get; set; }

         // Relação inversa (1 user → muitas expenses)
        public ICollection<Expense> Expenses { get; set; }  
    }
}