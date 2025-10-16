using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Domain.Entities
{
    public record Address(
        [property: JsonPropertyName("street")]
        [Required]
        string street = "",

        [property: JsonPropertyName("city")]
        [Required]
        string city = "",

        [property: JsonPropertyName("state")]
        [Required]
        string state = "",

        [property: JsonPropertyName("postal_code")]
        [Required]
        [RegularExpression(@"^([A-Z]{1}\d{4}[A-Z]{3})$", ErrorMessage = "Invalid postal code format")]
        string postal_code = "",

        [property: JsonPropertyName("country")]
        [Required]
        string country = ""
    );
}
