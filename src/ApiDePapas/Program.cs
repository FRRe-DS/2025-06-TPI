using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;
using ApiDePapas.Infrastructure.Persistence;
using ApiDePapas.Infrastructure.Repositories;
using ApiDePapas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.MigrationsAssembly("ApiDePapas.Infrastructure")));

// Para habilitar Swagger / OpenAPI (documentación interactiva)
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

// --- CONFIGURACIÓN KEYCLOAK JWT START ---
var keycloakAuthority = builder.Configuration["Keycloak:Authority"];
var keycloakAudience = builder.Configuration["Keycloak:Audience"];
var requireHttpsMetadata = builder.Configuration.GetValue<bool>("Keycloak:RequireHttpsMetadata");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = keycloakAuthority;
    options.Audience = keycloakAudience;
    options.RequireHttpsMetadata = requireHttpsMetadata;
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = keycloakAuthority,
        ValidateAudience = true,
        ValidAudience = keycloakAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for: {context.Principal?.Identity?.Name}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    // Política: requiere rol "logistica-be"
    options.AddPolicy("LogisticaBackend", policy =>
        policy.RequireClaim("realm_access.roles", "logistica-be"));
    
    // Política: requiere scope "envios:read"
    options.AddPolicy("EnviosRead", policy =>
        policy.RequireClaim("scope", "envios:read"));
    
    // Política: requiere scope "envios:write"
    options.AddPolicy("EnviosWrite", policy =>
        policy.RequireClaim("scope", "envios:write"));
});
// --- CONFIGURACIÓN KEYCLOAK JWT END ---

// --- ADD CORS CONFIGURATION START ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Allow your frontend origin
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// --- ADD CORS CONFIGURATION END ---

//Registro de servicios
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<IStockService, ApiDePapas.Application.Services.FakeStockService>();
builder.Services.AddScoped<TransportService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
// comento esto porq se usa para devolver un shipping no DB builder.Services.AddScoped<IShippingStore, ShippingStore>();
builder.Services.AddSingleton<IDistanceService, DistanceServiceInMemory>(); 
builder.Services.AddScoped<ILocalityRepository, LocalityRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ITravelRepository, TravelRepository>();           
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Tu store in-memory:
builder.Services.AddSingleton<IShippingStore, ApiDePapas.Infrastructure.ShippingStore>();
// Nota: singleton para que persista en memoria mientras corre la app
// (si la reiniciás, se pierde todo, claro)

// --- CONFIGURACIÓN HTTP CLIENTS PARA APIS EXTERNAS START ---
// Servicio de tokens de Keycloak
builder.Services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>();

// Cliente HTTP para API de Compras
var comprasApiBaseUrl = builder.Configuration["ExternalApis:ComprasApi:BaseUrl"] ?? "http://localhost:8081";
var comprasApiTimeout = builder.Configuration.GetValue<int>("ExternalApis:ComprasApi:Timeout", 30);

builder.Services.AddHttpClient<IComprasApiClient, ComprasApiClient>(client =>
{
    client.BaseAddress = new Uri(comprasApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(comprasApiTimeout);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Cliente HTTP para API de Stock
var stockApiBaseUrl = builder.Configuration["ExternalApis:StockApi:BaseUrl"] ?? "http://localhost:8082";
var stockApiTimeout = builder.Configuration.GetValue<int>("ExternalApis:StockApi:Timeout", 30);

builder.Services.AddHttpClient<IStockApiClient, StockApiClient>(client =>
{
    client.BaseAddress = new Uri(stockApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(stockApiTimeout);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
// --- CONFIGURACIÓN HTTP CLIENTS PARA APIS EXTERNAS END ---

var app = builder.Build();

// Configurar pipeline HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// --- USE CORS MIDDLEWARE START ---
app.UseCors("AllowFrontend"); // Apply the CORS policy
// --- USE CORS MIDDLEWARE END ---

// --- USE AUTHENTICATION & AUTHORIZATION START ---
app.UseAuthentication(); // IMPORTANTE: Antes de UseAuthorization
app.UseAuthorization();
// --- USE AUTHENTICATION & AUTHORIZATION END ---

app.MapControllers(); //clave para los controllers(?)

app.Run();


