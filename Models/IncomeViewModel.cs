using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class IncomeViewModel 
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Initial { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor tem de ser maior que zero")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

    }
}