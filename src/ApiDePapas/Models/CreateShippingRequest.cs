using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models

{
    public class CreateShippingRequest
    {
        //order_id es lo nuevo
        [Required]
        public int order_id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public Address delivery_address { get; set; }

        [Required]
        public string departure_postal_code { get; set; }

        [Required]
        public TransportType transport_type { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one product is required")]
        public List<ProductInput> product { get; set; }
    }
}