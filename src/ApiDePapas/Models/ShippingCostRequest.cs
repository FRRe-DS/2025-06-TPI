using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Models
{
    public class ShippingCostRequest
    {
        [Required]
        public Address delivery_address { get; set; }

        //en el nuevo yaml no piden esto
        //[Required]
        //public string departure_postal_code { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one product is required")]
        
        //OJO, esto debería ser ProductInput, lo dejo así para poder probar
        //Explicación: cost lo sacamos del get que hacemos a stock, donde obtenemos stats del producto
        //entienden o no cabrones?
        public List<ProductOutput> products { get; set; }
    }
}