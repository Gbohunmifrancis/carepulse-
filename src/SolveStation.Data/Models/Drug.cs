using SolveStation.Common.Models;

namespace SolveStation.Data.Models;

public record DrugId(Guid Value)
{
    public static DrugId NewId() => new(Guid.NewGuid());
    public static DrugId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();
}

public class Drug : BaseEntity<DrugId>
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public string Supplier { get; private set; }
    public string? BatchNumber { get; private set; }
    public string? DrugCode { get; private set; }

    private Drug() : base() { }

    public Drug(DrugId id, string name, string description, decimal price,
                int stockQuantity, int minimumStockLevel, DateTime expiryDate,
                string supplier, string? batchNumber = null, string? drugCode = null) : base()
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        MinimumStockLevel = minimumStockLevel;
        ExpiryDate = expiryDate;
        Supplier = supplier;
        BatchNumber = batchNumber;
        DrugCode = drugCode;

        AddDomainEvent(new DrugCreatedEvent(id, name, stockQuantity));

        if (IsLowStock())
        {
            AddDomainEvent(new LowStockAlertEvent(id, name, stockQuantity, minimumStockLevel));
        }
    }

    public void UpdateStock(int newQuantity, string reason, UserId updatedBy)
    {
        var previousQuantity = StockQuantity;
        StockQuantity = newQuantity;
        MarkAsUpdated();

        AddDomainEvent(new StockUpdatedEvent(Id, Name, previousQuantity, newQuantity, reason, updatedBy));

        if (IsLowStock())
        {
            AddDomainEvent(new LowStockAlertEvent(Id, Name, StockQuantity, MinimumStockLevel));
        }
    }

    public void UpdatePrice(decimal newPrice, UserId updatedBy)
    {
        var previousPrice = Price;
        Price = newPrice;
        MarkAsUpdated();

        AddDomainEvent(new DrugPriceUpdatedEvent(Id, Name, previousPrice, newPrice, updatedBy));
    }

    public void UpdateExpiryDate(DateTime newExpiryDate)
    {
        ExpiryDate = newExpiryDate;
        MarkAsUpdated();

        if (IsNearExpiry())
        {
            AddDomainEvent(new DrugExpiryAlertEvent(Id, Name, ExpiryDate));
        }
    }

    public bool IsLowStock() => StockQuantity <= MinimumStockLevel;
    public bool IsOutOfStock() => StockQuantity <= 0;
    public bool IsExpired() => ExpiryDate <= DateTime.UtcNow;
    public bool IsNearExpiry() => ExpiryDate <= DateTime.UtcNow.AddDays(30);

    public void ReduceStock(int quantity)
    {
        if (quantity > StockQuantity)
            throw new InvalidOperationException("Insufficient stock quantity");

        StockQuantity -= quantity;
        MarkAsUpdated();

        if (IsLowStock())
        {
            AddDomainEvent(new LowStockAlertEvent(Id, Name, StockQuantity, MinimumStockLevel));
        }
    }
}
