using FluentValidation;
using SolveStation.PrescriptionManagement.Models;

namespace SolveStation.PrescriptionManagement.Validators;

public class CreatePrescriptionRequestValidator : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required")
            .Must(BeValidGuid).WithMessage("Patient ID must be a valid GUID");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required")
            .Must(BeValidGuid).WithMessage("Doctor ID must be a valid GUID");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one prescription item is required")
            .Must(x => x.Count <= 20).WithMessage("Cannot have more than 20 items per prescription");

        RuleForEach(x => x.Items).SetValidator(new CreatePrescriptionItemRequestValidator());
    }

    private static bool BeValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }
}

public class CreatePrescriptionItemRequestValidator : AbstractValidator<CreatePrescriptionItemRequest>
{
    public CreatePrescriptionItemRequestValidator()
    {
        RuleFor(x => x.DrugId)
            .NotEmpty().WithMessage("Drug ID is required")
            .Must(BeValidGuid).WithMessage("Drug ID must be a valid GUID");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

        RuleFor(x => x.Instructions)
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");
    }

    private static bool BeValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }
}

public class UpdatePrescriptionRequestValidator : AbstractValidator<UpdatePrescriptionRequest>
{
    public UpdatePrescriptionRequestValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one prescription item is required")
            .Must(x => x.Count <= 20).WithMessage("Cannot have more than 20 items per prescription");

        RuleForEach(x => x.Items).SetValidator(new UpdatePrescriptionItemRequestValidator());
    }
}

public class UpdatePrescriptionItemRequestValidator : AbstractValidator<UpdatePrescriptionItemRequest>
{
    public UpdatePrescriptionItemRequestValidator()
    {
        RuleFor(x => x.DrugId)
            .NotEmpty().WithMessage("Drug ID is required")
            .Must(BeValidGuid).WithMessage("Drug ID must be a valid GUID");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantity cannot exceed 1000");

        RuleFor(x => x.Instructions)
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");

        RuleFor(x => x.ItemId)
            .Must(BeValidGuidOrNull).WithMessage("Item ID must be a valid GUID when provided");
    }

    private static bool BeValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }

    private static bool BeValidGuidOrNull(string? guid)
    {
        return string.IsNullOrEmpty(guid) || Guid.TryParse(guid, out _);
    }
}

public class FillPrescriptionRequestValidator : AbstractValidator<FillPrescriptionRequest>
{
    public FillPrescriptionRequestValidator()
    {
        RuleFor(x => x.PharmacistId)
            .NotEmpty().WithMessage("Pharmacist ID is required")
            .Must(BeValidGuid).WithMessage("Pharmacist ID must be a valid GUID");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item must be filled");

        RuleForEach(x => x.Items).SetValidator(new FillPrescriptionItemRequestValidator());
    }

    private static bool BeValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }
}

public class FillPrescriptionItemRequestValidator : AbstractValidator<FillPrescriptionItemRequest>
{
    public FillPrescriptionItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required")
            .Must(BeValidGuid).WithMessage("Item ID must be a valid GUID");

        RuleFor(x => x.QuantityFilled)
            .GreaterThan(0).WithMessage("Quantity filled must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Quantity filled cannot exceed 1000");
    }

    private static bool BeValidGuid(string guid)
    {
        return Guid.TryParse(guid, out _);
    }
}

public class PrescriptionSearchRequestValidator : AbstractValidator<PrescriptionSearchRequest>
{
    public PrescriptionSearchRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .Must(BeValidGuidOrNull).WithMessage("Patient ID must be a valid GUID when provided");

        RuleFor(x => x.DoctorId)
            .Must(BeValidGuidOrNull).WithMessage("Doctor ID must be a valid GUID when provided");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");
    }

    private static bool BeValidGuidOrNull(string? guid)
    {
        return string.IsNullOrEmpty(guid) || Guid.TryParse(guid, out _);
    }
}
