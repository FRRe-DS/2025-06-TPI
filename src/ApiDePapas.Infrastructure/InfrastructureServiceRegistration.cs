using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using ApiDePapas.Infrastructure.Auth;
using ApiDePapas.Infrastructure.Persistence;
using ApiDePapas.Infrastructure.Repositories;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Repositories;

namespace ApiDePapas.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            // Base de datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.MigrationsAssembly("ApiDePapas.Infrastructure");
                        mySqlOptions.EnableStringComparisonTranslations();
                    }));
        

            services.AddScoped<ILocalityRepository, LocalityRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ITravelRepository, TravelRepository>();
            services.AddScoped<IShippingRepository, ShippingRepository>();

            services.AddScoped<ITokenService, KeycloakTokenService>();

            services.AddHttpClient("KeycloakClient");

            return services;
        }
    }
}
