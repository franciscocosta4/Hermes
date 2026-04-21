using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{   
    // podemos adicionar campos extra depois (o entutycore já tras campos na tabela AspNetUsers)
    
    // Nome completo do utilizador
    public string FullName { get; set; }

}