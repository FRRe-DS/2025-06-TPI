using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.DTOs
{
    // Representa el resumen de un envío en una lista.
    public record ShipmentSummary(
        [property: JsonPropertyName("shipping_id")]
        [property: Required]
        int? ShippingId,

        [property: JsonPropertyName("order_id")]
        [property: Required]
        int? OrderId,

        [property: JsonPropertyName("user_id")]
        [property: Required]
        int? UserId,

        [property: JsonPropertyName("products")]
        [property: Required]
        List<ProductQty> Products,

        // ⇩⇩ ahora como texto (snake_case)
        [property: JsonPropertyName("status")]
        [property: Required]
        string Status,

        [property: JsonPropertyName("transport_type")]
        [property: Required]
        string TransportType,

        [property: JsonPropertyName("estimated_delivery_at")]
        [property: Required]
        DateTime? EstimatedDeliveryAt,

        [property: JsonPropertyName("created_at")]
        [property: Required]
        DateTime? CreatedAt
    );
}
