using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ShippingCostRequest(
        [property: JsonPropertyName("delivery_address")]
        [property: Required]
        Address DeliveryAddress,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductRequest> Products
    );
}