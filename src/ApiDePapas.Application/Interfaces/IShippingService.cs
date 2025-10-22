// En IShippingService.cs
using System;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Domain.Entities;
using System.Threading.Tasks; // Usaremos async para el futuro

namespace ApiDePapas.Application.Interfaces
{
    public interface IShippingService
    {
        // El servicio tomará el request y devolverá el response ya creado
        Task<CreateShippingResponse?> CreateNewShipping(CreateShippingRequest request);
        Task<ShippingDetail?> GetAsync(int id);                    // lectura desde DB
        Task<CancelShippingResponse> CancelAsync(int id, DateTime whenUtc);  // cancelar en DB
        Task<ShippingListResponse> List(int? userId,ShippingStatus? status,DateOnly? fromDate,DateOnly? toDate,int page,int limit);
    }
}