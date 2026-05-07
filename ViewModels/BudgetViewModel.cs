using System.ComponentModel.DataAnnotations;
using Hermes.Models;

namespace Hermes.Models
{
    public class BudgetViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }    
        public decimal? MonthExpenseSum { get; set; }    
        public decimal? CurrentBudgetLimit { get; set; }    
        public int? CurrentBudgetId { get; set; }    
        public decimal? DiffBudgetToExpense{ get; set; }    
        public decimal BudgetUsedPercentage{ get; set; }    
        public bool? isOverspending{ get; set; }    
    
        public List<Budget>? AllBudgets { get; set; }


    }
}