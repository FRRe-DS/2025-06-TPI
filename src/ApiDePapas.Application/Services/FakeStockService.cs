using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.ValueObjects;

// Se debería mover a un proyecto de pruebas distinto

namespace ApiDePapas.Application.Services;

public class FakeStockService : IStockService
{
    public Task<ProductDetail> GetProductDetailAsync(ProductQty product)
    {
        // ejemplo simple, con datos fijos para simular

        var detail = new ProductDetail
        {
            id = product.id,
            weight = 20,
            length = 10,
            width = 5,
            height = 2,
        };

        return Task.FromResult(detail);
    }
}