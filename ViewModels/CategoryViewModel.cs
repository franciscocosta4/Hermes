using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class CategoryViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Initial { get; set; }

        [Required]
        public  string Name { get; set; }

    }
}