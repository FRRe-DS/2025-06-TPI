using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.Entities;

public class Travel
{
    [Required]
    public int travel_id { get; set; }

    [Required]
    public DateTime departure_time { get; set; } // Hora de inicio del viaje

    public DateTime? arrival_time { get; set; } // Hora de llegada (opcional, puede ser nulo)

    [Required]
    public int transport_method_id { get; set; }
    public TransportMethod TransportMethod { get; set; } = null!; // Propiedad de navegación

    [Required]
    public int distribution_center_id { get; set; }
    public DistributionCenter DistributionCenter { get; set; } = null!; // Propiedad de navegación
    
    public ICollection<ShippingDetail> Shippings { get; set; } = new List<ShippingDetail>();
}