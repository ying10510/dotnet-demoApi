using System.Diagnostics;

namespace DemoApi.Middlewares;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = httpContext.Request;
        var traceId = httpContext.TraceIdentifier;

        _logger.LogInformation($"[{traceId}] => HTTP {request.Method} {request.Path} started");

        try
        {
            await _next(httpContext);
        }
        finally
        {
            stopwatch.Stop();
            var response = httpContext.Response;

            _logger.LogInformation($"[{traceId}] <= HTTP {request.Method} {request.Path} completed with {response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
        }

    }
}