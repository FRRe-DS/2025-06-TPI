using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record Address(
        [property: JsonPropertyName("street")]
        [property: Required]
        string street,

        [property: JsonPropertyName("city")]
        [property: Required]
        string city,

        [property: JsonPropertyName("state")]
        [property: Required]
        string state,

        [property: JsonPropertyName("postal_code")]
        [property: Required]
        [property: RegularExpression(@"^([A-Z]{1}\d{4}[A-Z]{3})$", ErrorMessage = "Invalid postal code format")]
        string postal_code,

        [property: JsonPropertyName("country")]
        [property: Required]
        string country
    );
}