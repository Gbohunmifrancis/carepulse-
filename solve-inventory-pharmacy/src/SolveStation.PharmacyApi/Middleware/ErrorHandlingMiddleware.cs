using System.Net;
using System.Text.Json;
using SolveStation.Common.Exceptions;
using FluentValidation;

namespace SolveStation.PharmacyApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();

        switch (exception)
        {
            case NotFoundException ex:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = ex.Message;
                errorResponse.Type = "Not Found";
                break;

            case UnauthorizedException ex:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = ex.Message;
                errorResponse.Type = "Authorization Error";
                break;

            case FluentValidation.ValidationException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Validation failed";
                errorResponse.Type = "Validation Error";
                errorResponse.Errors = ex.Errors?.ToDictionary(x => x.PropertyName, x => x.ErrorMessage);
                break;

            case DomainException ex:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = ex.Message;
                errorResponse.Type = "Domain Error";
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                errorResponse.Type = "Authorization Error";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                errorResponse.Type = "Internal Server Error";
                break;
        }

        errorResponse.StatusCode = response.StatusCode;
        errorResponse.Timestamp = DateTime.UtcNow;

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, string>? Errors { get; set; }
}
