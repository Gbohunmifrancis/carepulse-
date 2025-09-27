using SolveStation.Common.Interfaces;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public interface IPrescriptionRepository : IRepository<Prescription, PrescriptionId>
{
    // Explicit base repository methods for clarity
    Task<Prescription?> GetByIdAsync(PrescriptionId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Prescription> AddAsync(Prescription entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Prescription entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(PrescriptionId id, CancellationToken cancellationToken = default);
    
    // Specific prescription repository methods
    Task<IEnumerable<Prescription>> GetByPatientIdAsync(UserId patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetByDoctorIdAsync(UserId doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetPendingPrescriptionsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetExpiredPrescriptionsAsync(CancellationToken cancellationToken = default);
    Task<Prescription?> GetWithItemsAsync(PrescriptionId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Prescription>> GetFilledByPharmacistAsync(UserId pharmacistId, CancellationToken cancellationToken = default);
}
