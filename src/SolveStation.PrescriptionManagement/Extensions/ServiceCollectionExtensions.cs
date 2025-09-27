using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SolveStation.PrescriptionManagement.Services;
using SolveStation.PrescriptionManagement.Validators;

namespace SolveStation.PrescriptionManagement.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrescriptionManagement(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<IPrescriptionService, PrescriptionService>();

        // Register validators
        services.AddValidatorsFromAssemblyContaining<CreatePrescriptionRequestValidator>();

        return services;
    }
}
