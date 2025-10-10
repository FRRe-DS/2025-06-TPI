using System.Collections.Generic;
using ApiDePapas.Models;

namespace ApiDePapas.Services
{
    public interface IStockService
    {
        ProductDetail GetProductDetail(ProductQty product);
    }
}
