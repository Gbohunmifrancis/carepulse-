using SolveStation.Data.Models;

namespace SolveStation.Data.Specifications;

public class DrugSpecifications
{
    public class LowStockDrugs : BaseSpecification<Drug>
    {
        public LowStockDrugs()
        {
            SetCriteria(d => d.IsActive && d.StockQuantity <= d.MinimumStockLevel);
            ApplyOrderBy(d => d.StockQuantity);
        }
    }

    public class ExpiredDrugs : BaseSpecification<Drug>
    {
        public ExpiredDrugs()
        {
            var currentDate = DateTime.UtcNow;
            SetCriteria(d => d.IsActive && d.ExpiryDate <= currentDate);
            ApplyOrderBy(d => d.ExpiryDate);
        }
    }

    public class ExpiringDrugs : BaseSpecification<Drug>
    {
        public ExpiringDrugs(int daysFromNow = 30)
        {
            var currentDate = DateTime.UtcNow;
            var expiryThreshold = currentDate.AddDays(daysFromNow);
            SetCriteria(d => d.IsActive && d.ExpiryDate > currentDate && d.ExpiryDate <= expiryThreshold);
            ApplyOrderBy(d => d.ExpiryDate);
        }
    }

    public class DrugsBySupplier : BaseSpecification<Drug>
    {
        public DrugsBySupplier(string supplier)
        {
            SetCriteria(d => d.IsActive && d.Supplier.ToLower() == supplier.ToLower());
            ApplyOrderBy(d => d.Name);
        }
    }

    public class DrugSearch : BaseSpecification<Drug>
    {
        public DrugSearch(string searchTerm)
        {
            var lowerSearchTerm = searchTerm.ToLower();
            SetCriteria(d => d.IsActive &&
                (d.Name.ToLower().Contains(lowerSearchTerm) ||
                 d.Description.ToLower().Contains(lowerSearchTerm) ||
                 (d.DrugCode != null && d.DrugCode.ToLower().Contains(lowerSearchTerm))));
            ApplyOrderBy(d => d.Name);
        }
    }

    public class OutOfStockDrugs : BaseSpecification<Drug>
    {
        public OutOfStockDrugs()
        {
            SetCriteria(d => d.IsActive && d.StockQuantity <= 0);
            ApplyOrderBy(d => d.Name);
        }
    }

    public class DrugsByPriceRange : BaseSpecification<Drug>
    {
        public DrugsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            SetCriteria(d => d.IsActive && d.Price >= minPrice && d.Price <= maxPrice);
            ApplyOrderBy(d => d.Price);
        }
    }
}
