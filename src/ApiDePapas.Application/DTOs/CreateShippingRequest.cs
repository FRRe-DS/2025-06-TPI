using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.DTOs
{
    public record CreateShippingRequest(
        [property: JsonPropertyName("order_id")]
        [property: Required]
        int? order_id,

        [property: JsonPropertyName("user_id")]
        [property: Required]
        int? user_id,

        [property: JsonPropertyName("delivery_address")]
        [property: Required]
        Address delivery_address,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        TransportType? transport_type,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductRequest> products
    );
}