using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingService
    {
        ShippingListResponse List(
            int? userId,
            ShippingStatus? status,
            DateOnly? fromDate,
            DateOnly? toDate,
            int page,
            int limit);
    }
} 