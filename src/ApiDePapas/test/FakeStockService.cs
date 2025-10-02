using System.Collections.Generic;
using ApiDePapas.Models;
using ApiDePapas.Services;

namespace ApiDePapas.Test
{
    public class FakeStockService : IStockService
    {
        public List<ProductDetail> GetProductsDetail(List<ProductInput> products)
        {
            var result = new List<ProductDetail>();

            foreach (var p in products)
            {
                // devolvemos datos ficticios por producto
                result.Add(new ProductDetail
                {
                    id = p.id,
                    base_price = p.id * 100, // ejemplo simple, solo para simular
                    weight = 20,
                    length = 10,   // fijo para prueba
                    width = 5,
                    height = 2,
                });
            }

            return result;
        }
    }
}
