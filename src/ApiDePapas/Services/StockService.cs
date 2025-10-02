using System.Collections.Generic;
using ApiDePapas.Models;

namespace ApiDePapas.Services
{
    public interface IStockService
    {
        List<ProductDetail> GetProductsDetail(List<ProductInput> products);
    }
}
