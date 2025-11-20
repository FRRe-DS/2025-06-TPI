using System.Text.Json;
using System.Text.Json.Serialization;

using ApiDePapas.Utils;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;
using ApiDePapas.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddApiControllersAndSwagger();

// Cors configuration
builder.Services.AddApiCors();

// JWT Authentication configuration
builder.Services.AddJwtAuthentication(builder.Configuration);

//Registro de servicios
builder.Services.AddHttpClient<IStockService, StockService>();
builder.Services.AddHttpClient<IPurchasingService, PurchasingService>();

// Registro de servicios
builder.Services.AddApplicationServices();

builder.Services.AddScoped<ICalculateCost, CalculateCost>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IShippingStore, ShippingStore>();
builder.Services.AddHttpClient("KeycloakClient"); // HttpClient genérico para Keycloak

var app = builder.Build();

// Inicialización de base de datos
await DatabaseInitializer.InitializeDatabaseAsync(app.Services);

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();