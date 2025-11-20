using Microsoft.Extensions.DependencyInjection;

using ApiDePapas.Application.Services;
using ApiDePapas.Domain.Services;

namespace ApiDePapas.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<TransportService>();
            services.AddScoped<IShippingService, ShippingService>();
            services.AddScoped<IDistanceService, DistanceServiceInternal>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}