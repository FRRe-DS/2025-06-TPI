using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.DTOs;

public record ProductRequest(
    [property: JsonPropertyName("id")]
    [Required]
    int id,

    [property: JsonPropertyName("quantity")]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    int quantity
)
{
    public ProductQty ToProductQty() => new ProductQty(id, quantity);
}