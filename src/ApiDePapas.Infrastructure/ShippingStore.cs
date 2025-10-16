using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;
using System.Collections.Concurrent;

namespace ApiDePapas.Infrastructure
{
    public class ShippingStore : IShippingStore
    {
        private static int _nextId = 1;
        private static readonly ConcurrentDictionary<int, CreateShippingResponse> _db = new();

        public int Save(CreateShippingResponse response)
        {
            // 1. GENERA UN ID CONSTANTE O SECUENCIAL
            var newId = _nextId++;
            
            var stored = new CreateShippingResponse(
                newId,
                response.status,
                response.transport_type,
                response.estimated_delivery_at
            );
            _db[newId] = stored; // <-- guardamos en memoria
            // 2. "GUARDA" EL OBJETO ORIGINAL EN MEMORIA (OPCIONAL)
            // _inMemoryShippings.Add(response);
            
            // 3. DEVUELVE EL NUEVO ID
            return newId;
        }
        public CreateShippingResponse? GetById(int shippingId) // <-- NUEVO
        {
            return _db.TryGetValue(shippingId, out var value) ? value : null;
        }
        // Otros métodos necesarios para manejar envíos
        // Por ejemplo, actualizar estado, listar envíos, etc.
        //esto permite leer el envío por su ID
    }
}