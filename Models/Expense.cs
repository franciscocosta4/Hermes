using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//representa a tabela Expenses na base de dados
namespace Hermes.Models
{
    public class Expense
    {
        public int Id { get; set; }

        public string Description {get; set;} 
        public decimal Amount { get; set; }

        public DateOnly Date { get; set; }

        // Foreign key para o utilizador (AspNetUsers)
        public string UserId { get; set; }
        // Propriedade de navegação (many-to-one)
        public ApplicationUser User { get; set; }

        // Foreign key para a category 
        public int CategoryId { get; set; }

        // Propriedade de navegação (many-to-one)
        public Category Category { get; set; }
    }
}