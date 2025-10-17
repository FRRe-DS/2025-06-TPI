using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiDePapas.Domain.Entities
{
    public class DistributionCenter
        {
            // Clave primaria
            public int distribution_center_id { get; set; } 

            // Referencia a su Objeto de Valor (Address)
            public Address distribution_center_address { get; set; } = new Address(); 
            
    }
}