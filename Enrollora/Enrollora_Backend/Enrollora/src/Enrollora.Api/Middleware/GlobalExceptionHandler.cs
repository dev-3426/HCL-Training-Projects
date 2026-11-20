using System.Net;
using System.Text.Json;

namespace Enrollora.Api.Middleware;

public sealed class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled API exception.");

            var (status, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Operation could not be completed"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";

            var payload = new
            {
                type = "about:blank",
                title,
                status,
                detail = status == (int)HttpStatusCode.InternalServerError
                    ? "Please contact the system administrator."
                    : exception.Message,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
