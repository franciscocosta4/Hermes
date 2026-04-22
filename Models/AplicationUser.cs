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

    }
}