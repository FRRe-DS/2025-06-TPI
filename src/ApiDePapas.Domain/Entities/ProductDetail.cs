using System.ComponentModel.DataAnnotations;
using System.Numerics;

//deberiamos borrar esto, no tenemos esos datos (los pedimos a stock)
//lo dejo como clase de testing, para probar lo que nos debería devolver stock.

namespace ApiDePapas.Domain.Entities
{
    public class ProductDetail
    {
        [Required]
        public int id { get; set; }

        [Required]
        public int base_price { get; set; }

        [Required]
        public float weight { get; set; }

        [Required]
        public float length { get; set; }

        [Required]
        public float width { get; set; }

        [Required]
        public float height { get; set; }
    }
}