using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class ShippingLog
    {
        [Required]
        public string timestamp { get; set; }

        [Required]
        public ShippingStatus status { get; set; }

        [Required]
        public string message { get; set; }
    }
}