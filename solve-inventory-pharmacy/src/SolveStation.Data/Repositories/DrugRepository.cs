using Microsoft.EntityFrameworkCore;
using SolveStation.Data.Context;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public class DrugRepository : BaseRepository<Drug, DrugId>, IDrugRepository
{
    public DrugRepository(PharmacyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Drug>> GetLowStockDrugsAsync(CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(d => d.StockQuantity <= d.MinimumStockLevel)
            .OrderBy(d => d.StockQuantity)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Drug>> GetExpiredDrugsAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        return await GetActiveEntities()
            .Where(d => d.ExpiryDate <= currentDate)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Drug>> GetExpiringDrugsAsync(int daysFromNow = 30, CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        var expiryThreshold = currentDate.AddDays(daysFromNow);

        return await GetActiveEntities()
            .Where(d => d.ExpiryDate > currentDate && d.ExpiryDate <= expiryThreshold)
            .OrderBy(d => d.ExpiryDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Drug?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Drug>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(d => d.Name.ToLower().Contains(searchTerm.ToLower()) ||
                       d.Description.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Drug>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(d => d.Supplier.ToLower() == supplier.ToLower())
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsNameUniqueAsync(string name, DrugId? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = GetActiveEntities().Where(d => d.Name.ToLower() == name.ToLower());

        if (excludeId != null)
        {
            query = query.Where(d => !d.Id.Equals(excludeId));
        }

        return !await query.AnyAsync(cancellationToken);
    }
}
