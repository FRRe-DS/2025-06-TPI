using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ShippingLog(
        [property: JsonPropertyName("timestamp")]
        [property: Required]
        DateTime? Timestamp,

        [property: JsonPropertyName("status")]
        [property: Required]
        ShippingStatus? Status,

        [property: JsonPropertyName("message")]
        [property: Required]
        string Message
    );
}