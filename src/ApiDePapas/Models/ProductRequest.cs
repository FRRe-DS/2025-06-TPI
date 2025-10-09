using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ProductRequest(
        [property: JsonPropertyName("id")]
        [property: Required]
        int? Id,

        [property: JsonPropertyName("quantity")]
        [property: Required]
        [property: Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        int? Quantity
    );
}