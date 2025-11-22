using System.ComponentModel.DataAnnotations;

using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Domain.Entities;

public class ShippingDetail
{
    [Required]
    public int shipping_id { get; set; }

    [Required]
    public int order_id { get; set; }

    [Required]
    public int user_id { get; set; }

    [Required]
    public int delivery_address_id { get; set; }
    public Address DeliveryAddress { get; set; } = null!; // Navegación

    [Required]
    public List<ProductQty> products { get; set; } = new List<ProductQty>();

    [Required]
    public ShippingStatus status { get; set; }

    [Required]
    public int travel_id { get; set; }
    public Travel Travel { get; set; } = null!; // Propiedad de navegación

    public string tracking_number { get; set; } = string.Empty;

    public string carrier_name { get; set; } = string.Empty;

    public float total_cost { get; set; }

    public string currency { get; set; } = string.Empty;

    [Required]
    public DateTime estimated_delivery_at { get; set; }

    [Required]
    public DateTime created_at { get; set; }

    [Required]
    public DateTime updated_at { get; set; }

    [Required]
    public List<ShippingLog> logs { get; set; } = new List<ShippingLog>();

    public ShippingDetail(
        int order_id, int user_id, int travel_id, int delivery_address_id, float total_cost,
        string currency, List<ProductQty> products, double estimated_days
    )
    {
        this.order_id = order_id;
        this.user_id = user_id;

        this.travel_id = travel_id;
        this.delivery_address_id = delivery_address_id;

        this.products = products;

        status = ShippingStatus.Created;
        this.total_cost = total_cost;
        this.currency = currency;
        created_at = DateTime.UtcNow;
        updated_at = DateTime.UtcNow;

        estimated_delivery_at = DateTime.UtcNow.AddDays(estimated_days);

        tracking_number = Guid.NewGuid().ToString();
        carrier_name = "PENDIENTE";
        logs = [new ShippingLog(ShippingStatus.Created, "Shipping created in DB.")];
    }

    public bool IsCancellable()
    {
        return !(status is ShippingStatus.Delivered || status is ShippingStatus.Canceled);
    }
}