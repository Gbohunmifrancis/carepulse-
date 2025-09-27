using SolveStation.Data.Models;

namespace SolveStation.PrescriptionManagement.Models;

public record CreatePrescriptionRequest(
    string PatientId,
    string DoctorId,
    string Notes,
    List<CreatePrescriptionItemRequest> Items);

public record CreatePrescriptionItemRequest(
    string DrugId,
    int Quantity,
    string Instructions);

public record UpdatePrescriptionRequest(
    string Notes,
    List<UpdatePrescriptionItemRequest> Items);

public record UpdatePrescriptionItemRequest(
    string? ItemId,
    string DrugId,
    int Quantity,
    string Instructions);

public record FillPrescriptionRequest(
    string PharmacistId,
    List<FillPrescriptionItemRequest> Items);

public record FillPrescriptionItemRequest(
    string ItemId,
    int QuantityFilled);

public record PrescriptionResponse(
    string Id,
    string PatientId,
    string DoctorId,
    string Status,
    string Notes,
    DateTime? FilledAt,
    string? FilledByPharmacistId,
    DateTime? ExpiresAt,
    decimal TotalAmount,
    List<PrescriptionItemResponse> Items,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record PrescriptionItemResponse(
    string Id,
    string DrugId,
    string DrugName,
    int QuantityPrescribed,
    int QuantityFilled,
    decimal UnitPrice,
    decimal TotalPrice,
    string Instructions,
    bool IsFullyFilled,
    int RemainingQuantity);

public record PrescriptionSummaryResponse(
    string Id,
    string PatientId,
    string DoctorId,
    string Status,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt,
    DateTime? ExpiresAt);

public record PrescriptionSearchRequest(
    string? PatientId,
    string? DoctorId,
    PrescriptionStatus? Status,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IncludeExpired = false,
    int Page = 1,
    int PageSize = 20);

public record PrescriptionSearchResponse(
    List<PrescriptionSummaryResponse> Prescriptions,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
