using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class Error
    {
        [Required]
        public string code { get; set; } = string.Empty;

        [Required]
        public string message { get; set; } = string.Empty;

        [Required]
        public string details { get; set; } = string.Empty;
        // details es diferente de como esta definido en el YAML
    }
}
