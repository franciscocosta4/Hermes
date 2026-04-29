namespace Hermes.Models
{
    public class TransactionsViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Initial { get; set; }

        // podemos usar DashboardTransactionViewModel na pagina porque os dados acabam por ser os mesmos 
        public List<DashboardTransactionViewModel>? Transactions { get; set; }

    }
}