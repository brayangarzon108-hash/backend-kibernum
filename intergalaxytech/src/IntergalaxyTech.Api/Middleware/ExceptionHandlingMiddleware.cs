using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IntergalaxyTech.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado");
            var (status, title, errors) = ex switch
            {
                ValidationException ve => ((int)HttpStatusCode.BadRequest, "Error de validacion", ve.Errors.Select(e => e.ErrorMessage).ToArray()),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, ex.Message, Array.Empty<string>()),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, ex.Message, Array.Empty<string>()),
                _ => ((int)HttpStatusCode.InternalServerError, "Error interno del servidor", Array.Empty<string>())
            };
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = errors.Length > 0 ? string.Join(" | ", errors) : null,
                Instance = context.Request.Path
            });
        }
    }
}
