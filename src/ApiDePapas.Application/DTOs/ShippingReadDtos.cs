using System.Text.Json.Serialization;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.DTOs;

// DTOs Anidados para Romper el Ciclo JSON y exponer solo lo necesario

// DTO para la ADDRESS (Reemplaza $ref: "#/components/schemas/Address")
// Nota: Usaremos este DTO para mapear delivery_address y departure_address
public class AddressReadDto
{
    public int address_id { get; set; } = 0;
    public string street { get; set; } = string.Empty;
    public int number { get; set; } = 0;
    public string postal_code { get; set; } = string.Empty;
    public string locality_name { get; set; } = string.Empty;

    public AddressReadDto() {}
    
    public AddressReadDto(Address? a)
    {
        address_id = a?.address_id ?? 0;
        street = a?.street ?? string.Empty;
        number = a?.number ?? 0;
        postal_code = a?.postal_code ?? string.Empty;
        locality_name = a?.locality_name ?? string.Empty;
    }
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
    // Identificación y FKs
    [JsonPropertyOrder(1)] public int shipping_id { get; set; }
    [JsonPropertyOrder(2)] public int order_id { get; set; }
    [JsonPropertyOrder(3)] public int user_id { get; set; }

    // Direcciones
    [JsonPropertyOrder(4)] public AddressReadDto delivery_address { get; set; } = new AddressReadDto();
    [JsonPropertyOrder(5)] public AddressReadDto departure_address { get; set; } = new AddressReadDto(); 
    
    // Colecciones y Status
    [JsonPropertyOrder(6)] public List<ProductQtyReadDto> products { get; set; } = new List<ProductQtyReadDto>();
    [JsonPropertyOrder(7)] public ShippingStatus status { get; set; }
    [JsonPropertyOrder(8)] public string transport_type { get; set; } = string.Empty; 

    // Detalles
    [JsonPropertyOrder(9)] public string tracking_number { get; set; } = string.Empty;
    [JsonPropertyOrder(10)] public string carrier_name { get; set; } = string.Empty;
    [JsonPropertyOrder(11)] public float total_cost { get; set; }
    [JsonPropertyOrder(12)] public string currency { get; set; } = string.Empty;

    // Fechas
    [JsonPropertyOrder(13)] public DateTime estimated_delivery_at { get; set; }
    [JsonPropertyOrder(14)] public DateTime created_at { get; set; }
    [JsonPropertyOrder(15)] public DateTime updated_at { get; set; }
    [JsonPropertyOrder(16)] public List<ShippingLogReadDto> logs { get; set; } = new List<ShippingLogReadDto>();

    public ShippingDetailResponse(ShippingDetail s)
    {
        shipping_id = s.shipping_id;
        order_id = s.order_id;
        user_id = s.user_id;
        status = s.status;
        tracking_number = s.tracking_number;
        carrier_name = s.carrier_name;
        total_cost = s.total_cost;
        currency = s.currency;
        estimated_delivery_at = s.estimated_delivery_at;
        created_at = s.created_at;
        updated_at = s.updated_at;
        
        transport_type = s.Travel?.TransportMethod?.transport_type.ToString() ?? string.Empty;

        delivery_address = new AddressReadDto(s.DeliveryAddress);

        var departureAddressEntity = s.Travel?.DistributionCenter?.Address;
        departure_address = new AddressReadDto(departureAddressEntity);
        
        products = s.products.Select(p => new ProductQtyReadDto(product_id: p.id, quantity: p.quantity)).ToList();
        
        logs = s.logs.Select(l => new ShippingLogReadDto(
            timestamp: l.Timestamp, 
            status: l.Status, 
            message: l?.Message ?? string.Empty
        )).ToList();
    }

    public bool IsCancellable()
    {
        return !(status is ShippingStatus.Delivered || status is ShippingStatus.Canceled);
    }
}