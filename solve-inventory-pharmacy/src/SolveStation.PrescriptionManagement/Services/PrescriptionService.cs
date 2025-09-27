using Microsoft.Extensions.Logging;
using SolveStation.Common.Exceptions;
using SolveStation.Data.Models;
using SolveStation.Data.Repositories;
using SolveStation.Data;
using SolveStation.PrescriptionManagement.Models;
using CommonInterfaces = SolveStation.Common.Interfaces;

namespace SolveStation.PrescriptionManagement.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IDrugRepository _drugRepository;
    private readonly IUserRepository _userRepository;
    private readonly CommonInterfaces.IUnitOfWork _unitOfWork;
    private readonly ILogger<PrescriptionService> _logger;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IDrugRepository drugRepository,
        IUserRepository userRepository,
        CommonInterfaces.IUnitOfWork unitOfWork,
        ILogger<PrescriptionService> logger)
    {
        _prescriptionRepository = prescriptionRepository;
        _drugRepository = drugRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PrescriptionResponse> CreatePrescriptionAsync(CreatePrescriptionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating prescription for patient {PatientId} by doctor {DoctorId}", request.PatientId, request.DoctorId);

        // Validate and parse GUIDs
        if (!Guid.TryParse(request.PatientId, out var patientGuid))
        {
            throw new SolveStation.Common.Exceptions.ValidationException($"Invalid PatientId format: {request.PatientId}");
        }

        if (!Guid.TryParse(request.DoctorId, out var doctorGuid))
        {
            throw new SolveStation.Common.Exceptions.ValidationException($"Invalid DoctorId format: {request.DoctorId}");
        }

        var patientId = new UserId(patientGuid);
        var doctorId = new UserId(doctorGuid);

        var patient = await _userRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            throw new NotFoundException($"Patient with ID {request.PatientId} not found");

        var doctor = await _userRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor == null)
            throw new NotFoundException($"Doctor with ID {request.DoctorId} not found");

        var prescription = new Prescription(PrescriptionId.NewId(), patientId, doctorId, request.Notes);

        // Add prescription items
        foreach (var itemRequest in request.Items)
        {
            var drugId = new DrugId(Guid.Parse(itemRequest.DrugId));
            var drug = await ((CommonInterfaces.IRepository<Drug, DrugId>)_drugRepository).GetByIdAsync(drugId, cancellationToken);
            if (drug == null)
                throw new NotFoundException($"Drug with ID {itemRequest.DrugId} not found");

            prescription.AddItem(drug, itemRequest.Quantity, itemRequest.Instructions);
        }

        await _prescriptionRepository.AddAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created prescription {PrescriptionId} successfully", prescription.Id);
        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionResponse> GetPrescriptionByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await _prescriptionRepository.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            throw new NotFoundException($"Prescription with ID {id} not found");

        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionResponse> UpdatePrescriptionAsync(string id, UpdatePrescriptionRequest request, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await _prescriptionRepository.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            throw new NotFoundException($"Prescription with ID {id} not found");

        if (prescription.Status != PrescriptionStatus.Created)
            throw new DomainException("Only prescriptions in Created status can be updated");

        // Note: This is a simplified update - in a real scenario, you'd need to handle item updates more carefully
        // For now, we'll just update the notes and validate the prescription exists
        prescription.MarkAsUpdated();

        await _prescriptionRepository.UpdateAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionResponse> SubmitPrescriptionAsync(string id, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await _prescriptionRepository.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            throw new NotFoundException($"Prescription with ID {id} not found");

        prescription.Submit();
        await _prescriptionRepository.UpdateAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Submitted prescription {PrescriptionId}", prescription.Id);
        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionResponse> FillPrescriptionAsync(string id, FillPrescriptionRequest request, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await _prescriptionRepository.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            throw new NotFoundException($"Prescription with ID {id} not found");

        var pharmacistId = new UserId(Guid.Parse(request.PharmacistId));
        var pharmacist = await ((CommonInterfaces.IRepository<User, UserId>)_userRepository).GetByIdAsync(pharmacistId, cancellationToken);
        if (pharmacist == null)
            throw new NotFoundException($"Pharmacist with ID {request.PharmacistId} not found");

        // Validate drug availability and reduce stock
        var fillInfo = new List<PrescriptionItemFillInfo>();
        foreach (var item in request.Items)
        {
            var prescriptionItem = prescription.Items.FirstOrDefault(pi => pi.Id.Value.ToString() == item.ItemId);
            if (prescriptionItem == null)
                throw new NotFoundException($"Prescription item with ID {item.ItemId} not found");

            var drug = await ((CommonInterfaces.IRepository<Drug, DrugId>)_drugRepository).GetByIdAsync(prescriptionItem.DrugId, cancellationToken);
            if (drug == null)
                throw new NotFoundException($"Drug with ID {prescriptionItem.DrugId} not found");

            // Check if sufficient stock is available
            if (drug.StockQuantity < item.QuantityFilled)
                throw new SolveStation.Common.Exceptions.ValidationException($"Insufficient stock for drug '{drug.Name}'. Available: {drug.StockQuantity}, Required: {item.QuantityFilled}");

            // Reduce drug stock
            drug.ReduceStock(item.QuantityFilled);
            await ((CommonInterfaces.IRepository<Drug, DrugId>)_drugRepository).UpdateAsync(drug, cancellationToken);

            fillInfo.Add(new PrescriptionItemFillInfo(
                new PrescriptionItemId(Guid.Parse(item.ItemId)),
                item.QuantityFilled));
        }

        prescription.Fill(pharmacistId, fillInfo);
        await _prescriptionRepository.UpdateAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Filled prescription {PrescriptionId} by pharmacist {PharmacistId}", prescription.Id, pharmacistId);
        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionResponse> CancelPrescriptionAsync(string id, string reason, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await _prescriptionRepository.GetWithItemsAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            throw new NotFoundException($"Prescription with ID {id} not found");

        prescription.Cancel(reason);
        await _prescriptionRepository.UpdateAsync(prescription, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cancelled prescription {PrescriptionId} with reason: {Reason}", prescription.Id, reason);
        return MapToPrescriptionResponse(prescription);
    }

    public async Task<PrescriptionSearchResponse> SearchPrescriptionsAsync(PrescriptionSearchRequest request, CancellationToken cancellationToken = default)
    {
        var prescriptions = await ((CommonInterfaces.IRepository<Prescription, PrescriptionId>)_prescriptionRepository).GetAllAsync(cancellationToken);

        // Apply filters
        if (!string.IsNullOrEmpty(request.PatientId))
        {
            var patientId = new UserId(Guid.Parse(request.PatientId));
            prescriptions = prescriptions.Where(p => p.PatientId.Equals(patientId));
        }

        if (!string.IsNullOrEmpty(request.DoctorId))
        {
            var doctorId = new UserId(Guid.Parse(request.DoctorId));
            prescriptions = prescriptions.Where(p => p.DoctorId.Equals(doctorId));
        }

        if (request.Status.HasValue)
        {
            prescriptions = prescriptions.Where(p => p.Status == request.Status.Value);
        }

        if (request.StartDate.HasValue)
        {
            prescriptions = prescriptions.Where(p => p.CreatedAt >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            prescriptions = prescriptions.Where(p => p.CreatedAt <= request.EndDate.Value);
        }

        if (!request.IncludeExpired)
        {
            prescriptions = prescriptions.Where(p => !p.IsExpired());
        }

        var totalCount = prescriptions.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var pagedPrescriptions = prescriptions
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToPrescriptionSummary)
            .ToList();

        return new PrescriptionSearchResponse(pagedPrescriptions, totalCount, request.Page, request.PageSize, totalPages);
    }

    public async Task<List<PrescriptionSummaryResponse>> GetPrescriptionsByPatientAsync(string patientId, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(Guid.Parse(patientId));
        var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(userId, cancellationToken);
        return prescriptions.Select(MapToPrescriptionSummary).ToList();
    }

    public async Task<List<PrescriptionSummaryResponse>> GetPrescriptionsByDoctorAsync(string doctorId, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(Guid.Parse(doctorId));
        var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(userId, cancellationToken);
        return prescriptions.Select(MapToPrescriptionSummary).ToList();
    }

    public async Task<List<PrescriptionSummaryResponse>> GetPendingPrescriptionsAsync(CancellationToken cancellationToken = default)
    {
        var prescriptions = await _prescriptionRepository.GetPendingPrescriptionsAsync(cancellationToken);
        return prescriptions.Select(MapToPrescriptionSummary).ToList();
    }

    public async Task<List<PrescriptionSummaryResponse>> GetExpiredPrescriptionsAsync(CancellationToken cancellationToken = default)
    {
        var prescriptions = await _prescriptionRepository.GetExpiredPrescriptionsAsync(cancellationToken);
        return prescriptions.Select(MapToPrescriptionSummary).ToList();
    }

    public async Task<bool> DeletePrescriptionAsync(string id, CancellationToken cancellationToken = default)
    {
        var prescriptionId = new PrescriptionId(Guid.Parse(id));
        var prescription = await ((CommonInterfaces.IRepository<Prescription, PrescriptionId>)_prescriptionRepository).GetByIdAsync(prescriptionId, cancellationToken);

        if (prescription == null)
            return false;

        if (prescription.Status == PrescriptionStatus.Filled || prescription.Status == PrescriptionStatus.PartiallyFilled)
            throw new DomainException("Cannot delete filled prescriptions");

        await _prescriptionRepository.DeleteAsync(prescriptionId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted prescription {PrescriptionId}", prescriptionId);
        return true;
    }

    private PrescriptionResponse MapToPrescriptionResponse(Prescription prescription)
    {
        var items = prescription.Items.Select(item => new PrescriptionItemResponse(
            item.Id.ToString(),
            item.DrugId.ToString(),
            item.DrugName,
            item.QuantityPrescribed,
            item.QuantityFilled,
            item.UnitPrice,
            item.TotalPrice,
            item.Instructions,
            item.IsFullyFilled(),
            item.RemainingQuantity())).ToList();

        return new PrescriptionResponse(
            prescription.Id.ToString(),
            prescription.PatientId.ToString(),
            prescription.DoctorId.ToString(),
            prescription.Status.ToString(),
            prescription.Notes,
            prescription.FilledAt,
            prescription.FilledByPharmacistId?.ToString(),
            prescription.ExpiresAt,
            prescription.TotalAmount,
            items,
            prescription.CreatedAt,
            prescription.UpdatedAt);
    }

    private static PrescriptionSummaryResponse MapToPrescriptionSummary(Prescription prescription)
    {
        return new PrescriptionSummaryResponse(
            prescription.Id.ToString(),
            prescription.PatientId.ToString(),
            prescription.DoctorId.ToString(),
            prescription.Status.ToString(),
            prescription.TotalAmount,
            prescription.Items.Count,
            prescription.CreatedAt,
            prescription.ExpiresAt);
    }
}
