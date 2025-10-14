using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Infrastructure
{
    public class ShippingStore : IShippingStore
    {
        private static int _nextId = 1;

        public int Save(CreateShippingResponse response)
        {
            // 1. GENERA UN ID CONSTANTE O SECUENCIAL
            var newId = _nextId++;
            
            // 2. "GUARDA" EL OBJETO ORIGINAL EN MEMORIA (OPCIONAL)
            // _inMemoryShippings.Add(response);
            
            // 3. DEVUELVE EL NUEVO ID
            return newId;
        }
    }
}