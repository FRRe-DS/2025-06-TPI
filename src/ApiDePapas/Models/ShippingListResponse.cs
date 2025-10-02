using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class Shipments
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public int order_id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public List<ProductInput> products { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

        [Required]
        public string estimated_delivery_at { get; set; }

        [Required]
        public string created_at { get; set; }
    }
    public class Pagination
    {
        [Required]
        public int current_page { get; set; }

        [Required]
        public int total_pages { get; set; }

        [Required]
        public int total_items { get; set; }

        [Required]
        public int items_per_page { get; set; }
    }

    public class ShippingListResponse
    {
        [Required]
        public List<Shipments> shipments { get; set;}

        [Required]
        public Pagination pagination { get; set; }
    }
}