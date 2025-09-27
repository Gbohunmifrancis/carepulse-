using SolveStation.Common.Interfaces;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public interface IDrugRepository : IRepository<Drug, DrugId>
{
    // Explicit base repository methods for clarity
    Task<Drug?> GetByIdAsync(DrugId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Drug>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Drug> AddAsync(Drug entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Drug entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DrugId id, CancellationToken cancellationToken = default);
    
    // Specific drug repository methods
    Task<IEnumerable<Drug>> GetLowStockDrugsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Drug>> GetExpiredDrugsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Drug>> GetExpiringDrugsAsync(int daysFromNow = 30, CancellationToken cancellationToken = default);
    Task<Drug?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Drug>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Drug>> GetBySupplierAsync(string supplier, CancellationToken cancellationToken = default);
    Task<bool> IsNameUniqueAsync(string name, DrugId? excludeId = null, CancellationToken cancellationToken = default);
}
