using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.DTOs
{
    public record ShippingCostRequest(
        [property: JsonPropertyName("delivery_address")]
        [property: Required]
        Address delivery_address,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductQty> products
    );
}
