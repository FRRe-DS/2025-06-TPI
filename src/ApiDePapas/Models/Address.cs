using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record Address(
        [property: JsonPropertyName("street")]
        [property: Required]
        string Street,

        [property: JsonPropertyName("city")]
        [property: Required]
        string City,

        [property: JsonPropertyName("state")]
        [property: Required]
        string State,

        [property: JsonPropertyName("postal_code")]
        [property: Required]
        [property: RegularExpression(@"^([A-Z]{1}\d{4}[A-Z]{3})$", ErrorMessage = "Invalid postal code format")]
        string PostalCode,

        [property: JsonPropertyName("country")]
        [property: Required]
        string Country
    );
}