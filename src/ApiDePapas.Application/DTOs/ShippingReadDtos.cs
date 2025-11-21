using System.Text.Json.Serialization;

using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.DTOs;

// DTOs Anidados para Romper el Ciclo JSON y exponer solo lo necesario

// DTO para la ADDRESS (Reemplaza $ref: "#/components/schemas/Address")
// Nota: Usaremos este DTO para mapear delivery_address y departure_address
public class AddressReadDto
{
    public int address_id { get; set; }
    public string street { get; set; } = string.Empty;
    public int number { get; set; }
    public string postal_code { get; set; } = string.Empty;
    public string locality_name { get; set; } = string.Empty;
    // Aquí podrías agregar más campos de Locality si son esenciales, ej: state_name
}

public class TransportMethodReadDto
{
    public int transport_id { get; set; }
    public string transport_type { get; set; } = string.Empty;
    // Solo las propiedades básicas del transporte
}

public record ProductQtyReadDto(
    [property: JsonPropertyName("product_id")] int product_id, 
    [property: JsonPropertyName("quantity")] int quantity
);

public record ShippingLogReadDto(DateTime timestamp, ShippingStatus status, string message);

// DTO FINAL: ShippingDetailResponse (Coincide con el esquema ShippingDetail del YAML)
public class ShippingDetailResponse
{
    // Grupo 1: Identificación y FKs
    [JsonPropertyOrder(1)] public int shipping_id { get; set; }
    [JsonPropertyOrder(2)] public int order_id { get; set; }
    [JsonPropertyOrder(3)] public int user_id { get; set; }

    // Grupo 2: Direcciones
    [JsonPropertyOrder(4)] public AddressReadDto delivery_address { get; set; } = new AddressReadDto();
    [JsonPropertyOrder(5)] public AddressReadDto departure_address { get; set; } = new AddressReadDto(); 
    
    // Grupo 3: Colecciones y Status
    [JsonPropertyOrder(6)] public List<ProductQtyReadDto> products { get; set; } = new List<ProductQtyReadDto>();
    [JsonPropertyOrder(7)] public ShippingStatus status { get; set; }
    [JsonPropertyOrder(8)] public string transport_type { get; set; } = string.Empty; 

    // Grupo 4: Detalles
    [JsonPropertyOrder(9)] public string tracking_number { get; set; } = string.Empty;
    [JsonPropertyOrder(10)] public string carrier_name { get; set; } = string.Empty;
    [JsonPropertyOrder(11)] public float total_cost { get; set; }
    [JsonPropertyOrder(12)] public string currency { get; set; } = string.Empty;

    // Grupo 5: Fechas
    [JsonPropertyOrder(13)] public DateTime estimated_delivery_at { get; set; }
    [JsonPropertyOrder(14)] public DateTime created_at { get; set; }
    [JsonPropertyOrder(15)] public DateTime updated_at { get; set; }
    [JsonPropertyOrder(16)] public List<ShippingLogReadDto> logs { get; set; } = new List<ShippingLogReadDto>();

    public bool IsCancellable()
    {
        return !(status == ShippingStatus.Delivered || status == ShippingStatus.Canceled);
    }
}