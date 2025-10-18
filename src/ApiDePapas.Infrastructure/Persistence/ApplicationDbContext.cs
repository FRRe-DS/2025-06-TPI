using ApiDePapas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ApiDePapas.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // 1. DbSet para las Entidades Principales (Tablas)
        public DbSet<ShippingDetail> Shippings { get; set; } = null!;
        public DbSet<DistributionCenter> DistributionCenters { get; set; } = null!;
        public DbSet<TransportMethod> TransportMethods { get; set; } = null!;
        public DbSet<Travel> Travels { get; set; } = null!;
        public DbSet<Locality> Localities { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; } = null!; // Address es ahora una tabla

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 2. Definición de Claves Primarias
            modelBuilder.Entity<ShippingDetail>().HasKey(s => s.shipping_id);
            modelBuilder.Entity<TransportMethod>().HasKey(t => t.transport_id);
            modelBuilder.Entity<DistributionCenter>().HasKey(dc => dc.distribution_center_id); 
            modelBuilder.Entity<Travel>().HasKey(t => t.travel_id);
            modelBuilder.Entity<Address>().HasKey(a => a.address_id); // PK de Address
            modelBuilder.Entity<Locality>().HasKey(l => new {l.postal_code, l.locality_name}); // Clave Compuesta
            
            // 3. Mapeo de Relaciones de Entidades

            // A. Relación ShippingDetail (N) a Travel (1)
            modelBuilder.Entity<ShippingDetail>()
                .HasOne(s => s.Travel) 
                .WithMany(t => t.Shippings)
                .HasForeignKey(s => s.travel_id)
                .IsRequired();
            
            // B. Relación Travel (N) a TransportMethod (1)
            modelBuilder.Entity<Travel>()
                .HasOne(t => t.TransportMethod) 
                .WithMany()
                .HasForeignKey(t => t.transport_method_id)
                .IsRequired();
            
            // C. Relación Travel (N) a DistributionCenter (1)
            modelBuilder.Entity<Travel>()
                .HasOne(t => t.DistributionCenter)
                .WithMany()
                .HasForeignKey(t => t.distribution_center_id)
                .IsRequired();
            
            // D. Relación ShippingDetail (N) a Address (1) - SOLO ENTREGA
            modelBuilder.Entity<ShippingDetail>()
                .HasOne(s => s.DeliveryAddress) // Propiedad de navegación 'DeliveryAddress'
                .WithMany(a => a.DeliveredShippings) // Colección en Address
                .HasForeignKey(s => s.delivery_address_id) // Clave foránea en ShippingDetail
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            // E. Relación Address (N) a Locality (1)
            modelBuilder.Entity<Address>()
                .HasOne(a => a.Locality) // Address tiene UNA Locality
                .WithMany() // Locality no necesita navegar de vuelta a Address
                .HasForeignKey(a => new { a.postal_code, a.locality_name }) // Usa la FK compuesta
                .IsRequired();

            // 4. Mapeo de Propiedades Secundarias
            
            // A. Colecciones (Tablas Satélite)
            modelBuilder.Entity<ShippingDetail>().OwnsMany(s => s.products);
            modelBuilder.Entity<ShippingDetail>().OwnsMany(s => s.logs);

            // B. Dirección dentro de DistributionCenter (como Value Object)
            // Ya que DistributionCenter sigue siendo una clase con su propia PK, esta es la forma de incrustar su dirección.
            modelBuilder.Entity<DistributionCenter>()
                    .HasOne(dc => dc.Address) // Centro tiene UNA Dirección
                    .WithMany() // No necesitamos navegar de vuelta desde Address a DistributionCenter
                    .HasForeignKey(dc => dc.address_id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}