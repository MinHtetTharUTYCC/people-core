using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace PeopleCore.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = new
        {
            success = false,
            message = "An unexpected error occurred.",
            errors = new[] { new { field = "General", message = exception.Message } }
        };

        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case KeyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new
                {
                    success = false,
                    message = "Resource not found.",
                    errors = new[] { new { field = "General", message = exception.Message } }
                };
                break;
            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new
                {
                    success = false,
                    message = "Invalid argument.",
                    errors = new[] { new { field = "General", message = exception.Message } }
                };
                break;
            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
