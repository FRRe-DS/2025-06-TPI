using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;
using ApiDePapas.Infrastructure;
using ApiDePapas.Infrastructure.Persistence; // <-- 1. Añadimos el 'using' de la nueva clase
using ApiDePapas.Infrastructure.Repositories;
using ApiDePapas.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
// Ya no necesitamos 'MySqlConnector' aquí

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
builder.Services.AddHttpClient<IStockService, StockService>();
builder.Services.AddHttpClient<IPurchasingService, PurchasingService>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<TransportService>();
builder.Services.AddScoped<IShippingService, ShippingService>();
builder.Services.AddScoped<IDistanceService, DistanceServiceInternal>();
builder.Services.AddScoped<ILocalityRepository, LocalityRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ITravelRepository, TravelRepository>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddSingleton<IShippingStore, ApiDePapas.Infrastructure.ShippingStore>();

var app = builder.Build();

// --- 2. ¡MIRA QUÉ LIMPIO! ---
// Toda la lógica de inicialización ahora está en este método de extensión
await DatabaseInitializer.InitializeDatabaseAsync(app.Services);
// --- FIN DEL CAMBIO ---

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();

// 3. Todo el código 'static async Task InitializeDatabaseAsync...'
//    y 'static async Task LoadCsvDataAsync...'
//    ha desaparecido de este archivo.