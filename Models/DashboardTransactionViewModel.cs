public class DashboardTransactionViewModel
{
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public required string Type { get; set; } // "Income" ou "Expense"

    public string? Category {get; set;}
}