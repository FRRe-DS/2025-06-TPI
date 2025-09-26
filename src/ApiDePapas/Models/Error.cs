using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class Error
    {
        [Required]
        public string code { get; set; }

        [Required]
        public string message { get; set; }

        [Required]
        public string details { get; set; }
        // details es diferente de como esta definido en el YAML
    }
}
