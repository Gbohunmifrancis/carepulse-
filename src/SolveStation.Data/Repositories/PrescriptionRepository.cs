using Microsoft.EntityFrameworkCore;
using SolveStation.Data.Context;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public class PrescriptionRepository : BaseRepository<Prescription, PrescriptionId>, IPrescriptionRepository
{
    public PrescriptionRepository(PharmacyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(UserId patientId, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.PatientId.Equals(patientId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(UserId doctorId, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.DoctorId.Equals(doctorId))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetByStatusAsync(PrescriptionStatus status, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetPendingPrescriptionsAsync(CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.Status == PrescriptionStatus.Pending)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetExpiredPrescriptionsAsync(CancellationToken cancellationToken = default)
    {
        var currentDate = DateTime.UtcNow;
        return await GetActiveEntities()
            .Where(p => p.ExpiresAt.HasValue && p.ExpiresAt.Value <= currentDate &&
                       p.Status != PrescriptionStatus.Filled && p.Status != PrescriptionStatus.Cancelled)
            .OrderBy(p => p.ExpiresAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Prescription?> GetWithItemsAsync(PrescriptionId id, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Include(p => p.Items)
                .ThenInclude(pi => pi.Drug)
            .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Prescription>> GetFilledByPharmacistAsync(UserId pharmacistId, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(p => p.FilledByPharmacistId != null && p.FilledByPharmacistId.Equals(pharmacistId))
            .OrderByDescending(p => p.FilledAt)
            .ToListAsync(cancellationToken);
    }
}
