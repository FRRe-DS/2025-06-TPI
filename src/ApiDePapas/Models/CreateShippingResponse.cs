using System.Collections.Generic;
using System.ComponentModel.DataAnotations;

namespace ApiDePapas.Models
{
    public class CreateShippingResponse
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

        [Required]
        public string estimated_delivery_at { get; set; }
    }
}