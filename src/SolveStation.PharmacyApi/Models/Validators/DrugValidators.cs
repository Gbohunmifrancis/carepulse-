using FluentValidation;
using SolveStation.PharmacyApi.Models.DTOs;

namespace SolveStation.PharmacyApi.Models.Validators;

public class CreateDrugRequestValidator : AbstractValidator<CreateDrugRequest>
{
    public CreateDrugRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Drug name is required")
            .MaximumLength(100).WithMessage("Drug name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThan(0).WithMessage("Minimum stock level must be greater than 0");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.Today).WithMessage("Expiry date must be in the future");

        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Supplier is required")
            .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters");

        RuleFor(x => x.BatchNumber)
            .MaximumLength(50).WithMessage("Batch number cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));

        RuleFor(x => x.DrugCode)
            .MaximumLength(20).WithMessage("Drug code cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.DrugCode));
    }
}

public class UpdateDrugRequestValidator : AbstractValidator<UpdateDrugRequest>
{
    public UpdateDrugRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Drug name is required")
            .MaximumLength(100).WithMessage("Drug name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.MinimumStockLevel)
            .GreaterThan(0).WithMessage("Minimum stock level must be greater than 0");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.Today).WithMessage("Expiry date must be in the future");

        RuleFor(x => x.Supplier)
            .NotEmpty().WithMessage("Supplier is required")
            .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters");

        RuleFor(x => x.BatchNumber)
            .MaximumLength(50).WithMessage("Batch number cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.BatchNumber));

        RuleFor(x => x.DrugCode)
            .MaximumLength(20).WithMessage("Drug code cannot exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.DrugCode));
    }
}

public class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequest>
{
    public UpdateStockRequestValidator()
    {
        RuleFor(x => x.NewQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason for stock update is required")
            .MaximumLength(200).WithMessage("Reason cannot exceed 200 characters");
    }
}
