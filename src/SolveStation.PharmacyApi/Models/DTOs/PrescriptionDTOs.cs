namespace SolveStation.PharmacyApi.Models.DTOs;

// Prescription DTOs
public record PrescriptionDto : BaseDto
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateTime? FilledAt { get; init; }
    public Guid? FilledByPharmacistId { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public decimal TotalAmount { get; init; }
    public List<PrescriptionItemDto> Items { get; init; } = new();
    public UserDto? Patient { get; init; }
    public UserDto? Doctor { get; init; }
    public UserDto? FilledByPharmacist { get; init; }
}

public record PrescriptionItemDto : BaseDto
{
    public Guid DrugId { get; init; }
    public int Quantity { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init; }
    public DrugDto? Drug { get; init; }
}

public record CreatePrescriptionRequest
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public string Notes { get; init; } = string.Empty;
    public List<CreatePrescriptionItemRequest> Items { get; init; } = new();
}

public record CreatePrescriptionItemRequest
{
    public Guid DrugId { get; init; }
    public int Quantity { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
}

public record UpdatePrescriptionRequest
{
    public string Notes { get; init; } = string.Empty;
    public List<UpdatePrescriptionItemRequest> Items { get; init; } = new();
}

public record UpdatePrescriptionItemRequest
{
    public Guid? Id { get; init; }
    public Guid DrugId { get; init; }
    public int Quantity { get; init; }
    public string Dosage { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
}

public record FillPrescriptionRequest
{
    public Guid PharmacistId { get; init; }
    public List<FillPrescriptionItemRequest> Items { get; init; } = new();
}

public record FillPrescriptionItemRequest
{
    public Guid ItemId { get; init; }
    public int QuantityFilled { get; init; }
}

public record PrescriptionSearchRequest
{
    public Guid? PatientId { get; init; }
    public Guid? DoctorId { get; init; }
    public string? Status { get; init; }
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
