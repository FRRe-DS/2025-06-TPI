using ApiDePapas.Services;
var builder = WebApplication.CreateBuilder(args);

// Para habilitar Swagger / OpenAPI (documentación interactiva)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registro de servicios
builder.Services.AddScoped<ICalculateCost, CalculateCost>();

var app = builder.Build();

// Configurar pipeline HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agregar autenticación (opcional)

app.Run();


