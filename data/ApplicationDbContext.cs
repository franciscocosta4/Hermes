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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Income -> User
            builder.Entity<Income>()
                .HasOne(i => i.User)
                .WithMany(u => u.Incomes)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category -> User
            builder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Expense -> User
            builder.Entity<Expense>()
                .HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Expense -> Category
            builder.Entity<Expense>()
                .HasOne(e => e.Category)
                .WithMany(c => c.Expenses)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}