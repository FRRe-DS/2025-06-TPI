using ApiDePapas.Application.Interfaces;
using ApiDePapas.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Para habilitar Swagger / OpenAPI (documentación interactiva)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registro de servicios
builder.Services.AddScoped<ICalculateCost, CalculateCost>();
builder.Services.AddScoped<IStockService, ApiDePapas.Application.Services.FakeStockService>();

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


