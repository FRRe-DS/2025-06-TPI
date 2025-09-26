using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace ApiDePapas.Models
{
    public class ProductQty
    {
        [Required]
        public BigInteger Product_id { get; set; }

        [Required]
        public BigInteger quantity { get; set; }
    }
}