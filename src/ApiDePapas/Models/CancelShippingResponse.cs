using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class CancelShippingResponse
    {
        [Required]
        public int shipping_id { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public DateTime cancelled_at { get; set; }
    }
}