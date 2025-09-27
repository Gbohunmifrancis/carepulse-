namespace SolveStation.PharmacyApi.Models.DTOs;

/// <summary>
/// Generic paginated result wrapper
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public record PagedResult<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}
