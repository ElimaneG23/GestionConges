using GestionConges.API.Extensions;
using GestionConges.API.Middleware;
using GestionConges.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- Services (voir GestionConges.API/Extensions/ServiceCollectionExtensions.cs) ----
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestion des Congés — API SaaS",
        Version = "v1",
        Description = "API multi-tenant pour la gestion des congés (SuperAdmin, Admin entreprise, Manager, Employé)."
    });

    // Support du Bearer token dans Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Entrez uniquement le token JWT (sans le préfixe 'Bearer ')."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---- Pipeline HTTP ----
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Applique automatiquement les migrations en développement (pratique pour démarrer vite).
if (app.Environment.IsDevelopment() && builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception exception)
    {
        scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseStartup")
            .LogError(exception, "Database migration failed. The API is starting, but database-backed endpoints may be unavailable.");
    }
}

app.Run();
