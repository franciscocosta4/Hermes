using System.ComponentModel.DataAnnotations;

namespace Hermes.Models
{
    public class CategoryViewModel
    {
        // passamos os dados que são necessários para a sidebar:
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Initial { get; set; }

        [Required]
        public string Name { get; set; }

    }
}