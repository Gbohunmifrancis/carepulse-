using SolveStation.PrescriptionManagement.Models;
using SolveStation.Data.Models;

namespace SolveStation.PrescriptionManagement.Services;

public interface IPrescriptionService
{
    Task<PrescriptionResponse> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken cancellationToken = default);
    Task<PrescriptionResponse> GetPrescriptionByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<PrescriptionResponse> UpdatePrescriptionAsync(string id, UpdatePrescriptionRequest request, CancellationToken cancellationToken = default);
    Task<PrescriptionResponse> SubmitPrescriptionAsync(string id, CancellationToken cancellationToken = default);
    Task<PrescriptionResponse> FillPrescriptionAsync(string id, FillPrescriptionRequest request, CancellationToken cancellationToken = default);
    Task<PrescriptionResponse> CancelPrescriptionAsync(string id, string reason, CancellationToken cancellationToken = default);
    Task<PrescriptionSearchResponse> SearchPrescriptionsAsync(PrescriptionSearchRequest request, CancellationToken cancellationToken = default);
    Task<List<PrescriptionSummaryResponse>> GetPrescriptionsByPatientAsync(string patientId, CancellationToken cancellationToken = default);
    Task<List<PrescriptionSummaryResponse>> GetPrescriptionsByDoctorAsync(string doctorId, CancellationToken cancellationToken = default);
    Task<List<PrescriptionSummaryResponse>> GetPendingPrescriptionsAsync(CancellationToken cancellationToken = default);
    Task<List<PrescriptionSummaryResponse>> GetExpiredPrescriptionsAsync(CancellationToken cancellationToken = default);
    Task<bool> DeletePrescriptionAsync(string id, CancellationToken cancellationToken = default);
}
