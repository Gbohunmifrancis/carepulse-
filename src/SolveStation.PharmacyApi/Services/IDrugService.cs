using SolveStation.PharmacyApi.Models.DTOs;

namespace SolveStation.PharmacyApi.Services;

public interface IDrugService
{
    Task<PagedResult<DrugDto>> SearchDrugsAsync(DrugSearchRequest request, CancellationToken cancellationToken = default);
    Task<DrugDto> GetDrugByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DrugDto> CreateDrugAsync(CreateDrugRequest request, CancellationToken cancellationToken = default);
    Task<DrugDto> UpdateDrugAsync(Guid id, UpdateDrugRequest request, CancellationToken cancellationToken = default);
    Task<DrugDto> UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken cancellationToken = default);
    Task<List<DrugDto>> GetLowStockDrugsAsync(CancellationToken cancellationToken = default);
    Task<List<DrugDto>> GetExpiredDrugsAsync(CancellationToken cancellationToken = default);
    Task<bool> DeleteDrugAsync(Guid id, CancellationToken cancellationToken = default);
}