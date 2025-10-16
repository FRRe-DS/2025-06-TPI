using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;
using ApiDePapas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Base de datos
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.MigrationsAssembly("ApiDePapas.Infrastructure")));

// Para habilitar Swagger / OpenAPI (documentación interactiva)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registro de servicios
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<IStockService, ApiDePapas.Application.Services.FakeStockService>();
builder.Services.AddScoped<TransportService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IShippingStore, ShippingStore>();

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


