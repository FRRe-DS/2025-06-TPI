using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.DTOs;

public record ShippingCostRequest(
    [property: JsonPropertyName("delivery_address")]
    [Required]
    DeliveryAddressRequest delivery_address,

    [property: JsonPropertyName("products")]
    [Required]
    List<ProductQty> products
) {
    public ShippingCostRequest(CreateShippingRequest csr) : this(
        csr.delivery_address,
        csr.products.Select(p => new ProductQty(p.id, p.quantity)).ToList()
    ) {}
}