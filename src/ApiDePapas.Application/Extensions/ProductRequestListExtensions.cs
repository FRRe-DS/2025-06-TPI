using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.Extensions;

public static class ProductRequestListExtension
{
    public static List<ProductQty> ToProductQtyList(this List<ProductRequest> pr) =>
        [.. pr.Select(p => new ProductQty(p.id, p.quantity))];
}