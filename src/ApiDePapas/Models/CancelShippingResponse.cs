using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record CancelShippingResponse(
        [property: JsonPropertyName("shipping_id")]
        [property: Required]
        int? ShippingId,

        [property: JsonPropertyName("status")]
        [property: Required]
        ShippingStatus? Status,

        [property: JsonPropertyName("cancelled_at")]
        [property: Required]
        DateTime? CancelledAt
    );
}