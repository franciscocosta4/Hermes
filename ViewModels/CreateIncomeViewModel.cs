using System.ComponentModel.DataAnnotations;

// usamos um viewmodel específico para o post para evitar de o create mandar os dados da IncomeViewModel, que têm Email, fullname ...
// e ia acabar por falhar no post : [{"Field":"Email","Errors":["The Email field is required."]},{"Field":"Initial","Errors":["The Initial field is required."]},{"Field":"FullName","Errors":["The FullName field is required."]}]
public class CreateIncomeViewModel
{

    [Required(ErrorMessage = "O valor é obrigatório")]
    [Range(0.01, 999999999, ErrorMessage = "O valor tem de ser maior que zero")]
    public decimal Amount { get; set; }

    [Required]
    public DateOnly Date { get; set; }
}