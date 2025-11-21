using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.Services
{
    // Nombre de la clase ajustado
    public class TransportService
    {
        public TransportMethodsResponse GetAll()
        {
            return new TransportMethodsResponse
            {
                transport_methods = new List<TransportMethods>
                {
                    // Usando los nuevos TransportType: truck, boat, plain
                    new() { type = TransportType.Truck, name = "Road Trucking", estimated_days = "3-7" },
                    new() { type = TransportType.Boat, name = "Sea Freight", estimated_days = "15-30" },
                    new() { type = TransportType.Plane, name = "Air Cargo", estimated_days = "1-3" },
                }
            };
        }
    }
}