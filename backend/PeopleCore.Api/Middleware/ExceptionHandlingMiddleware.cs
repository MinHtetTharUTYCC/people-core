using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace PeopleCore.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var isDevelopment = _env.IsDevelopment();
        var errorMessage = isDevelopment ? exception.Message : "An unexpected error occurred.";

        var response = new
        {
            success = false,
            message = "An unexpected error occurred.",
            errors = new[] { new { field = "General", message = errorMessage } }
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
                    errors = new[] { new { field = "General", message = errorMessage } }
                };
                break;
            case ArgumentException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new
                {
                    success = false,
                    message = "Invalid argument.",
                    errors = new[] { new { field = "General", message = errorMessage } }
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
