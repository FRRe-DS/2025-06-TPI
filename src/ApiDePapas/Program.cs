using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;           // <-- para ShippingStore
using ApiDePapas.Domain.Entities;        // <-- si hacés seed opcional

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de servicios
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IStockService, FakeStockService>();
builder.Services.AddSingleton<IShippingStore, ShippingStore>(); // repo en memoria

var app = builder.Build();

var store = app.Services.GetRequiredService<IShippingStore>() as ShippingStore;
store?.Seed(new[]
{
    new ShippingDetail {
        shipping_id = 1, order_id = 123, user_id = 456,
        products = new() { new ProductQty(12, 2), new ProductQty(22, 1) },
        status = ShippingStatus.in_transit,
        transport_type = TransportType.road,
        estimated_delivery_at = DateTime.UtcNow.AddDays(3),
        created_at = DateTime.UtcNow.AddDays(-2),
        updated_at = DateTime.UtcNow
    },
    new ShippingDetail {
        shipping_id = 2, order_id = 124, user_id = 456,
        products = new() { new ProductQty(31, 3) },
        status = ShippingStatus.delivered,         
        transport_type = TransportType.air,        
        estimated_delivery_at = DateTime.UtcNow.AddDays(-1),
        created_at = DateTime.UtcNow.AddDays(-10),
        updated_at = DateTime.UtcNow.AddDays(-1)
    }
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
