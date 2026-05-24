using FluentValidation;
using IntergalaxyTech.Api.Middleware;
using IntergalaxyTech.Application.Abstractions;
using IntergalaxyTech.Application.Services;
using IntergalaxyTech.Application.Validators;
using IntergalaxyTech.Infrastructure.Data;
using IntergalaxyTech.Infrastructure.External;
using IntergalaxyTech.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
builder.Services.AddScoped<IPersonajeRepository, PersonajeRepository>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<PersonajeService>();
builder.Services.AddScoped<SolicitudService>();
builder.Services.AddValidatorsFromAssemblyContaining<CrearSolicitudRequestValidator>();
builder.Services.AddHttpClient<IRickAndMortyClient, RickAndMortyClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["RickAndMorty:BaseUrl"]);
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
}); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
// Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin() // Permitir todas las solicitudes de origen. O usa WithOrigins("http://localhost:8080") para permitir solo este dominio.
               .AllowAnyMethod() // Permitir todos los métodos HTTP (GET, POST, etc.).
               .AllowAnyHeader(); // Permitir todos los encabezados.
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/health");
app.MapControllers();
// Usar la política CORS configurada
app.UseCors("CorsPolicy");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }
