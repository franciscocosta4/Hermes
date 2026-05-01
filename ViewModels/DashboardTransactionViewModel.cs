public class DashboardTransactionViewModel
{

    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public required string Type { get; set; } // "Income" ou "Expense"
    public  string Description { get; set; } 

    public string? Category {get; set;}

}