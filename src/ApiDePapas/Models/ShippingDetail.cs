using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class ShippingDetail
    {
        [Required]
        public int? shipping_id { get; set; }

        [Required]
        public int? order_id { get; set; }

        [Required]
        public int? user_id { get; set; }

        [Required]
        public Address delivery_address { get; set; }

        public Address departure_address { get; set; }

        [Required]
        public List<ProductQty> products { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

        public string tracking_number { get; set; }

        public string carrier_name { get; set; }

        public float total_cost { get; set; }

        public string currency { get; set; }

        [Required]
        public DateTime estimated_delivery_at { get; set; }

        [Required]
        public DateTime created_at { get; set; }

        [Required]
        public DateTime updated_at { get; set; }

        [Required]
        public List<ShippingLog> logs { get; set; }

    }
}