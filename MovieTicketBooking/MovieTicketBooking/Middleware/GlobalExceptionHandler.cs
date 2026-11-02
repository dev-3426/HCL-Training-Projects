
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Exceptions;

namespace MovieTicketBooking.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string errorCode;
        string message;

        if (exception is AppException appException)
        {
            statusCode = appException.StatusCode;
            errorCode = appException.ErrorCode;
            message = appException.Message;

            _logger.LogWarning(
                exception,
                "Application error {ErrorCode}: {Message}",
                errorCode,
                message);
        }
        else if (exception is InvalidOperationException)
        {
            statusCode = StatusCodes.Status400BadRequest;
            errorCode = "INVALID_OPERATION";
            message = exception.Message;

            _logger.LogWarning(
                exception,
                "Invalid business operation: {Message}",
                message);
        }
        else
        {
            statusCode = StatusCodes.Status500InternalServerError;
            errorCode = "INTERNAL_SERVER_ERROR";
            message = "An unexpected error occurred.";

            _logger.LogError(
                exception,
                "An unexpected error occurred while processing the request.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,

            Title = statusCode switch
            {
                400 => "Bad Request",
                404 => "Resource Not Found",
                409 => "Conflict",
                _ => "Internal Server Error"
            },

            Detail = message,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["errorCode"] = errorCode;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}
