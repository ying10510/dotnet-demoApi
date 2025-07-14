using DemoApi.Exceptions;
using DemoApi.Dtos;

namespace DemoApi.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogWarning("Business exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UnhandledException] Path: {Path}, TraceId: {TraceId}", context.Request.Path, context.TraceIdentifier);
            await HandleExceptionAsync(context, 500, "Internal server error", ex.StackTrace, context.TraceIdentifier);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, int statusCode, string message, string? details = null, string? traceId = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var errorResponse = new ErrorResponseDto
        {
            StatusCode = statusCode,
            Message = message,
            Details = details,
            TraceId = traceId ?? context.TraceIdentifier
        };

        return context.Response.WriteAsJsonAsync(errorResponse);
    }

}