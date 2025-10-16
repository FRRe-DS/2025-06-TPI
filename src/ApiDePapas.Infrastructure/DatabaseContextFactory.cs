using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ApiDePapas.Infrastructure.Persistence
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            // Cargar configuración
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Obtener la cadena de conexión
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //DEBUG opcional para confirmar que EF toma la cadena correcta
            Console.WriteLine($"[DEBUG] ConnectionString usada: {connectionString}");

            // Configurar el DbContext
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
