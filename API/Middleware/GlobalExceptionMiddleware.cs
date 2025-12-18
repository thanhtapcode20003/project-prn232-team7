using System.Net;
using System.Text.Json;
using BusinessObjectLayer.Exceptions;

namespace API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiError error = exception switch
        {
            NotFoundException notFoundEx => notFoundEx.Error,
            UnauthorizedException unauthorizedEx => unauthorizedEx.Error,
            ValidationException validationEx => validationEx.Error,
            ApiException apiEx => apiEx.Error,

            // EF Core exceptions
            Microsoft.EntityFrameworkCore.DbUpdateException dbEx =>
                ApiError.Conflict("Database update failed", _env.IsDevelopment() ? dbEx.InnerException?.Message : null),

            // Unauthorized access
            UnauthorizedAccessException _ =>
                ApiError.Unauthorized("You don't have permission to access this resource"),

            // Argument exceptions
            ArgumentNullException argNullEx =>
                ApiError.BadRequest($"Required parameter is missing: {argNullEx.ParamName}"),

            ArgumentException argEx =>
                ApiError.BadRequest(argEx.Message),

            // Default to 500. In development include full exception details (including inner exceptions)
            _ => ApiError.InternalServerError(
                _env.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                _env.IsDevelopment() ? exception.ToString() : null)
        };

        context.Response.StatusCode = error.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment()
        };

        var json = JsonSerializer.Serialize(error, options);
        await context.Response.WriteAsync(json);
    }
}

// Extension method để đăng ký middleware
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}