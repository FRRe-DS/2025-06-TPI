using ApiDePapas.Application.Services;
using ApiDePapas.Application.Interfaces;

namespace ApiDePapas.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<TransportService>();
        services.AddScoped<IShippingService, ShippingService>();
        services.AddScoped<IDistanceService, DistanceServiceInternal>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ICalculateCost, CalculateCost>();

        return services;
    }
}