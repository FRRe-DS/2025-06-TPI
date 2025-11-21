using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.ValueObjects;

namespace ApiDePapas.Application.Interfaces;
public interface IShippingService
{
    Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest request);

    Task<ShippingDetailResponse?> GetByIdAsync(int id);

    Task<CancelShippingResponse> CancelAsync(int id, DateTime whenUtc);
    
    Task<ShippingListResponse> List(int? userId,ShippingStatus? status,DateOnly? fromDate,DateOnly? toDate,int page,int limit);
}