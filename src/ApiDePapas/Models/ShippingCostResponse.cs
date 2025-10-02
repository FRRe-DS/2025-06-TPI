using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class ShippingCostResponse
    {
        [Required]
        public string currency { get; set; }

        [Required]
        public float total_cost { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

        [Required]
        public List<ProductOutput> products { get; set; }
    }
}