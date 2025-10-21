using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.Entities
{
    public class ShippingDetail
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public int order_id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public Address delivery_address { get; set; } = new Address();

        public Address departure_address { get; set; } = new Address();

        [Required]
        public List<ProductQty> products { get; set; } = new List<ProductQty>();

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

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
    }
}
 