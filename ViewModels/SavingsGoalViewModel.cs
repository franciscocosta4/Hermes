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

        
        // Todos os goals do utilizador
        public List<SavingsGoal> AllGoals { get; set; } = new List<SavingsGoal>();
        
        // Para mostrar alerts
        public bool IsCompleted { get; set; }
        public bool IsCloseToCompletion { get; set; } // menos de 10% a faltar
    }
}