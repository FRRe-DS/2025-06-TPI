using ApiDePapas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiDePapas.Infrastructure.Persistence;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    // Un DbSet representa una tabla en la base de datos.
    // Solo necesitamos uno para la entidad "raíz", que es ShippingDetail.
    public DbSet<ShippingDetail> ShippingDetails { get; set; }

    // Aquí es donde configuramos las relaciones y mapeos complejos.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración para la entidad ShippingDetail
        modelBuilder.Entity<ShippingDetail>(entity =>
        {
            // 1. Definir la tabla y la clave primaria
            entity.ToTable("ShippingDetails");
            entity.HasKey(e => e.shipping_id);

            // 2. Mapear los 'Address' como "Owned Types".
            // Esto evita crear una tabla separada para Address y en su lugar
            // crea columnas como "delivery_address_street", "delivery_address_city", etc.
            entity.OwnsOne(e => e.delivery_address);
            entity.OwnsOne(e => e.departure_address);

            // 3. Mapear las listas como "Owned Collections".
            // Esto creará tablas separadas para 'ProductQty' y 'ShippingLog'
            // que estarán fuertemente vinculadas a un ShippingDetail.
            entity.OwnsMany(e => e.products, product => {
                product.ToTable("ShippingProducts");
            });
            entity.OwnsMany(e => e.logs, log => {
                log.ToTable("ShippingLogs");
            });

            // 4. Convertir los Enums a strings en la base de datos para que sean legibles.
            entity.Property(e => e.status).HasConversion<string>();
            entity.Property(e => e.transport_method).HasConversion<string>();
        });
    }
}