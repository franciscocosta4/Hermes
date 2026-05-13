// Models/SavingsGoalViewModel.cs
using System.Collections.Generic;

namespace Hermes.Models
{
    public class SavingsGoalViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }   
        // Goal atual em foco (ou o primeiro ativo)
        public int? CurrentGoalId { get; set; }
        public string CurrentGoalName { get; set; }
        public decimal CurrentTargetAmount { get; set; }
        public decimal CurrentSavedAmount { get; set; }
        public int? CurrentPercentageOfIncome { get; set; }
        public decimal? CurrentMinimumBalanceToKeep { get; set; }
        
        // Percentagem atingida
        public decimal GoalProgressPercentage { get; set; }
        
        // Quanto falta
        public decimal RemainingAmount { get; set; }
        
        // Todos os goals do utilizador
        public List<SavingsGoal> AllGoals { get; set; } = new List<SavingsGoal>();
        
        // Para mostrar alerts
        public bool IsCompleted { get; set; }
        public bool IsCloseToCompletion { get; set; } // menos de 10% a faltar
    }
}