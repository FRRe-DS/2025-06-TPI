using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ApiDePapas.Models
{
    public class ProductQty
    {
        [Required]
        public int product_id { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}