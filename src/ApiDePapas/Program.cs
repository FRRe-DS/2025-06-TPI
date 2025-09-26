var builder = WebApplication.CreateBuilder(args);

// Para habilitar Swagger / OpenAPI (documentación interactiva)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar pipeline HTTP

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agregar autenticación (opcional)

// endpoints 


app.Run();

// modelos / records


