using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.DTOs
{
    public record CreateShippingResponse(
        [property: JsonPropertyName("shipping_id")]
        [property: Required]
        int? shipping_id,

        [property: JsonPropertyName("status")]
        [property: Required]
        ShippingStatus? status,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        TransportType? transport_type,

        [property: JsonPropertyName("estimated_delivery_at")]
        [property: Required]
        DateTime? estimated_delivery_at
    );
}