using System.Net;
using System.Text.Json;

namespace GestionConges.API.Middleware;

/// <summary>
/// Capture toute exception non gérée et retourne une réponse JSON homogène,
/// pour éviter de fuiter des stack traces au client.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur non gérée : {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var payload = JsonSerializer.Serialize(new
            {
                error = "Une erreur interne est survenue.",
                detail = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? ex.Message
                    : null
            });

            await context.Response.WriteAsync(payload);
        }
    }
}
