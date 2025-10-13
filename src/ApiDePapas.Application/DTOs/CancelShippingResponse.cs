using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.DTOs
{
    public record CancelShippingResponse(
        [property: JsonPropertyName("shipping_id")]
        [property: Required]
        int shipping_id,

        [property: JsonPropertyName("status")]
        [property: Required]
        ShippingStatus status,

        [property: JsonPropertyName("cancelled_at")]
        [property: Required]
        DateTime cancelled_at
    );
}
