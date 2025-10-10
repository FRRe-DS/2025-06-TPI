using System.Collections.Generic;
using ApiDePapas.Models;
using ApiDePapas.Services;

namespace ApiDePapas.Test
{
    public class FakeStockService : IStockService
    {
        public ProductDetail GetProductDetail(ProductQty product)
        {
            // ejemplo simple, con datos fijos para simular

            var detail = new ProductDetail
            { 
                    id = product.id,
                    base_price = product.id * 100,
                    weight = 20,
                    length = 10,
                    width = 5,
                    height = 2,
            };

            return detail;
        }
    }
}
