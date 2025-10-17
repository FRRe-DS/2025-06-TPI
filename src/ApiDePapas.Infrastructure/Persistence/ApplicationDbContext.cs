using ApiDePapas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ApiDePapas.Infrastructure.Persistence
{
    // Asegúrate de tener instalado el paquete NuGet adecuado para tu motor (ej: Pomelo.EntityFrameworkCore.MySql)
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 1. DbSet para las Entidades Principales (Tablas)
        public DbSet<ShippingDetail> Shippings { get; set; } = null!;
        public DbSet<DistributionCenter> DistributionCenters { get; set; } = null!;
        public DbSet<TransportMethod> TransportMethods { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 2. Definición de Claves Primarias
            modelBuilder.Entity<ShippingDetail>().HasKey(s => s.shipping_id);
            modelBuilder.Entity<TransportMethod>().HasKey(t => t.transport_id);
            // Asume que la clave en la clase es 'distribution_center_id'
            modelBuilder.Entity<DistributionCenter>().HasKey(dc => dc.distribution_center_id);

            // 3. Mapeo de Relaciones de Clave Foránea (N:1)

            // Relación ShippingDetail (N) a DistributionCenter (1)
            modelBuilder.Entity<ShippingDetail>()
                .HasOne<DistributionCenter>() // ShippingDetail tiene UN DistributionCenter
                .WithMany() // No queremos la propiedad de navegación inversa (la colección)
                .HasForeignKey(s => s.distribution_center_id) // Usa el ID de FK que definiste en ShippingDetail
                .IsRequired();
            
            // Relación ShippingDetail (N) a TransportMethod (1)
            modelBuilder.Entity<ShippingDetail>()
                .HasOne<TransportMethod>() // ShippingDetail tiene UN TransportMethod
                .WithMany() // No queremos la propiedad de navegación inversa (la colección)
                .HasForeignKey(s => s.transport_method_id) // Usa el ID de FK que definiste en ShippingDetail
                .IsRequired();

            // 4. Mapeo de Value Objects / Owned Entities (Incorporados o Tablas Satélite)
            
            // A. Direcciones Incorporadas (columnas en la tabla ShippingDetails)
            modelBuilder.Entity<ShippingDetail>().OwnsOne(s => s.delivery_address);
            modelBuilder.Entity<ShippingDetail>().OwnsOne(s => s.departure_address);
            
            // B. Colecciones (Tablas Satélite con Clave Foránea)
            modelBuilder.Entity<ShippingDetail>().OwnsMany(s => s.products); // Crea la tabla ProductQty (o ShippingDetail_products)
            modelBuilder.Entity<ShippingDetail>().OwnsMany(s => s.logs);     // Crea la tabla ShippingLog (o ShippingDetail_logs)

            // C. Dirección dentro de DistributionCenter (como Value Object)
            modelBuilder.Entity<DistributionCenter>().OwnsOne(dc => dc.distribution_center_address);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}