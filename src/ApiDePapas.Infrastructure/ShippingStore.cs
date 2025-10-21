using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using System.Collections.Concurrent;

namespace ApiDePapas.Infrastructure
{
    public class ShippingStore : IShippingStore
    {
        private readonly ConcurrentDictionary<int, ShippingDetail> _db = new();

        public IEnumerable<ShippingDetail> GetAll() => _db.Values;

        // Opcional para pruebas
        public void Seed(IEnumerable<ShippingDetail> items)
        {
            foreach (var s in items)
                _db.TryAdd(s.shipping_id, s);
        }
    }
}
