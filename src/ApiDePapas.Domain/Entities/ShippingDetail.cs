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


        //Acá debería ser TransportMethod que es un id de un transporte en específico (por ejemplo Camión ID 739)
        [Required]
        public int transport_method_id { get; set; }

        //Nuevo -> ojo que es ID en ambos porq usamos como clave foranea, tanto transport_method como distribution_center_id van a tener sus tablas
        [Required]
        public int distribution_center_id { get; set; }

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
