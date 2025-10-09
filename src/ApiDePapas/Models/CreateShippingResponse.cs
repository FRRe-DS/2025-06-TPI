using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record CreateShippingResponse(
        [property: JsonPropertyName("shipping_id")]
        [property: Required]
        int? ShippingId,

        [property: JsonPropertyName("status")]
        [property: Required]
        ShippingStatus? Status,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        TransportType? TransportType,

        [property: JsonPropertyName("estimated_delivery_at")]
        [property: Required]
        DateTime? EstimatedDeliveryAt
    );
}