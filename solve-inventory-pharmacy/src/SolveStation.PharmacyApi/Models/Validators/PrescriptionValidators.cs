using FluentValidation;
using SolveStation.PharmacyApi.Models.DTOs;

namespace SolveStation.PharmacyApi.Models.Validators;

public class CreatePrescriptionRequestValidator : AbstractValidator<CreatePrescriptionRequest>
{
    public CreatePrescriptionRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one prescription item is required")
            .Must(HaveValidItems).WithMessage("All prescription items must be valid");

        RuleForEach(x => x.Items)
            .SetValidator(new CreatePrescriptionItemRequestValidator());
    }

    private static bool HaveValidItems(List<CreatePrescriptionItemRequest> items)
    {
        return items.All(item => item.DrugId != Guid.Empty && item.Quantity > 0);
    }
}

public class CreatePrescriptionItemRequestValidator : AbstractValidator<CreatePrescriptionItemRequest>
{
    public CreatePrescriptionItemRequestValidator()
    {
        RuleFor(x => x.DrugId)
            .NotEmpty().WithMessage("Drug ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required")
            .MaximumLength(100).WithMessage("Dosage cannot exceed 100 characters");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required")
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");
    }
}

public class UpdatePrescriptionRequestValidator : AbstractValidator<UpdatePrescriptionRequest>
{
    public UpdatePrescriptionRequestValidator()
    {
        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one prescription item is required");

        RuleForEach(x => x.Items)
            .SetValidator(new UpdatePrescriptionItemRequestValidator());
    }
}

public class UpdatePrescriptionItemRequestValidator : AbstractValidator<UpdatePrescriptionItemRequest>
{
    public UpdatePrescriptionItemRequestValidator()
    {
        RuleFor(x => x.DrugId)
            .NotEmpty().WithMessage("Drug ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Dosage)
            .NotEmpty().WithMessage("Dosage is required")
            .MaximumLength(100).WithMessage("Dosage cannot exceed 100 characters");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Instructions are required")
            .MaximumLength(500).WithMessage("Instructions cannot exceed 500 characters");
    }
}

public class FillPrescriptionRequestValidator : AbstractValidator<FillPrescriptionRequest>
{
    public FillPrescriptionRequestValidator()
    {
        RuleFor(x => x.PharmacistId)
            .NotEmpty().WithMessage("Pharmacist ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item must be filled");

        RuleForEach(x => x.Items)
            .SetValidator(new FillPrescriptionItemRequestValidator());
    }
}

public class FillPrescriptionItemRequestValidator : AbstractValidator<FillPrescriptionItemRequest>
{
    public FillPrescriptionItemRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("Item ID is required");

        RuleFor(x => x.QuantityFilled)
            .GreaterThan(0).WithMessage("Quantity filled must be greater than 0");
    }
}
