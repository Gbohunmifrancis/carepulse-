using System.Diagnostics;

namespace SolveStation.PharmacyApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;

        _logger.LogInformation("Starting {Method} {Path} from {RemoteIpAddress}",
            request.Method,
            request.Path,
            context.Connection.RemoteIpAddress);

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation("Completed {Method} {Path} with {StatusCode} in {ElapsedMilliseconds}ms",
            request.Method,
            request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}
