namespace SolveStation.PharmacyApi.Models.DTOs;

// Drug DTOs
public record DrugDto : BaseDto
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public int MinimumStockLevel { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string? BatchNumber { get; init; }
    public string? DrugCode { get; init; }
    public bool IsLowStock { get; init; }
    public bool IsExpired { get; init; }
}

public record CreateDrugRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public int MinimumStockLevel { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string? BatchNumber { get; init; }
    public string? DrugCode { get; init; }
}

public record UpdateDrugRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int MinimumStockLevel { get; init; }
    public DateTime ExpiryDate { get; init; }
    public string Supplier { get; init; } = string.Empty;
    public string? BatchNumber { get; init; }
    public string? DrugCode { get; init; }
}

public record UpdateStockRequest
{
    public int NewQuantity { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public record DrugSearchRequest
{
    public string? Name { get; init; }
    public string? Supplier { get; init; }
    public bool? IsLowStock { get; init; }
    public bool? IsExpired { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
