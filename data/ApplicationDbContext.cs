using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hermes.Models;

namespace Hermes.Data
{
    // Herdamos de IdentityDbContext para incluir tabelas de autenticação
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Construtor recebe opções configuradas no Program.cs
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Aqui podemos adicionar outras tabelas da app
        // public DbSet<Produto> Produtos { get; set; }
        public DbSet<Income> Incomes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Configuração da relação many-to-one do income com o user
            builder.Entity<Income>()
                .HasOne(i => i.User)              // Um income tem um user
                .WithMany(u => u.Incomes)         // Um user tem muitos incomes
                .HasForeignKey(i => i.UserId)     // FK
                .OnDelete(DeleteBehavior.Cascade); // Opcional

        }

    }
}