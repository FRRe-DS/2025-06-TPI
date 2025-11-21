using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Domain.ValueObjects;

public record ShippingLog(
    [property: JsonPropertyName("timestamp")]
    [Required]
    DateTime? Timestamp,

    [property: JsonPropertyName("status")]
    [Required]
    ShippingStatus? Status,

    [property: JsonPropertyName("message")]
    [Required]
    string Message
);