using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    // este ViewModel serve para controlar o que sai da camada de dados, evita expor a entidade Income diretamente
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