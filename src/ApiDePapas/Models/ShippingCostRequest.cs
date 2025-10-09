using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Models
{
    public record ShippingCostRequest(
        [property: JsonPropertyName("delivery_address")]
        [property: Required]
        Address delivery_address,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductRequest> products
    );
}