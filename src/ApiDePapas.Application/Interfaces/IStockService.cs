using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.Interfaces;

public interface IStockService
{
    Task<ProductDetail> GetProductDetailAsync(ProductQty product);
}