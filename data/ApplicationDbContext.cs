using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeuProjetoMvc.Data
{
    // Herdamos de IdentityDbContext para incluir tabelas de autenticação
    public class ApplicationDbContext : IdentityDbContext
    {
        // Construtor recebe opções configuradas no Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Aqui podemos adicionar outras tabelas da tua app
        // public DbSet<Produto> Produtos { get; set; }
    }
}