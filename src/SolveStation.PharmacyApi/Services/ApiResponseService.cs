using SolveStation.PharmacyApi.Models.DTOs;

namespace SolveStation.PharmacyApi.Services;

/// <summary>
/// Service for generating API responses in a consistent format
/// </summary>
public interface IApiResponseService
{
    ApiResponse<T> Success<T>(T data, string message = "Success");
    ApiResponse<T> Error<T>(string message, int statusCode = 400, T? data = default);
    PagedApiResponse<T> PagedSuccess<T>(PagedResult<T> pagedData, string message = "Success");
}

public class ApiResponseService : IApiResponseService
{
    public ApiResponse<T> Success<T>(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    public ApiResponse<T> Error<T>(string message, int statusCode = 400, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Data = data,
            Timestamp = DateTime.UtcNow
        };
    }

    public PagedApiResponse<T> PagedSuccess<T>(PagedResult<T> pagedData, string message = "Success")
    {
        return new PagedApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = pagedData.Items,
            TotalCount = pagedData.TotalCount,
            Page = pagedData.Page,
            PageSize = pagedData.PageSize,
            TotalPages = pagedData.TotalPages,
            HasNext = pagedData.HasNext,
            HasPrevious = pagedData.HasPrevious,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Paginated API response wrapper
/// </summary>
public class PagedApiResponse<T> : ApiResponse<List<T>>
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
