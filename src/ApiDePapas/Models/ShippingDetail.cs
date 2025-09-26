using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class ShippingDetail
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public List<ProductQty> products { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public DateTime estimated_delivery_at { get; set; }

        [Required]
        public List<ShippingLog> logs { get; set; }

    }
}