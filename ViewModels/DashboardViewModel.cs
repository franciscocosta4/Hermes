namespace Hermes.Models
{
    public class DashboardViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Initial { get; set; }
        //aqui é definido o content da dashboard tipo :
        // public Float balance { get; set; }

        public decimal MonthIncomeSum { get; set; }
        public decimal MonthExpenseSum { get; set; }
        public decimal MonthBalance { get; set; }
        public decimal Sum90DaysExpenses { get; set; }

        public List<DashboardTransactionViewModel>? MonthTransactions { get; set; }

    }
}