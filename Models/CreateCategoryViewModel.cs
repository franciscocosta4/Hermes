using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class CreateCategoryViewModel
    {
        [Required]
        public string Name { get; set; }

    }
}