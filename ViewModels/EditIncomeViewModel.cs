using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class EditIncomeViewModel
    {
        // levam ? (nullable) porque este ViewModel é reutilizado em contexto de GET (preencher os inputs da pagina de edit)
        // e POST (atualizar na bd o income), onde não são enviados dados do user.
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Initial { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "The value is required")]
        [Range(0.01, 999999999, ErrorMessage = "The amount has to be greater than 0")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateOnly Date { get; set; }

    }
}