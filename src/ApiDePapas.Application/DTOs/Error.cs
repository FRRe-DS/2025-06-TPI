using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Application.DTOs
{
    public class Error
    {
        [Required]
        public string code { get; set; } = string.Empty;

        [Required]
        public string message { get; set; } = string.Empty; 

        [Required]
        public object? details { get; set; } 
        // details es diferente de como esta definido en el YAML, creo que ahora ya no es distinto (chequar)
    }
}
