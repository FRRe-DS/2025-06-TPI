using System.Collections.Generic;

using ApiDePapas.Domain.Entities;
using ApiDePapas.Application.DTOs;

namespace ApiDePapas.Application.Interfaces
{
    public interface IStockService
    {
        ProductDetail GetProductDetail(ProductQty product);
    }
}