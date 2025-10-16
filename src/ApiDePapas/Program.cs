using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Para habilitar Swagger / OpenAPI (documentación interactiva)
builder.Services.AddControllers();
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        );
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registro de servicios
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<IStockService, ApiDePapas.Application.Services.FakeStockService>();
builder.Services.AddScoped<TransportService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IShippingStore, ShippingStore>();

// Tu store in-memory:
builder.Services.AddSingleton<IShippingStore, ApiDePapas.Infrastructure.ShippingStore>();
// Nota: singleton para que persista en memoria mientras corre la app
// (si la reiniciás, se pierde todo, claro)

var app = builder.Build();

// Configurar pipeline HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); //clave para los controllers(?)

app.Run();


