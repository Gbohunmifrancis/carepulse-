using Microsoft.EntityFrameworkCore;
using SolveStation.Data.Context;
using SolveStation.Data.Models;

namespace SolveStation.Data.Seed;

public static class SeedData
{
    public static async Task SeedAsync(PharmacyDbContext context, Func<string, string>? hashPassword = null)
    {
        if (!await context.Users.AnyAsync())
        {
            await SeedUsersAsync(context, hashPassword);
        }

        if (!await context.Drugs.AnyAsync())
        {
            await SeedDrugsAsync(context);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(PharmacyDbContext context, Func<string, string>? hashPassword = null)
    {
        // Use the provided hash function or create simple hashed passwords for development
        string HashPasswordSafe(string password) => hashPassword?.Invoke(password) ?? $"hashed_{password}";
        
        var users = new List<User>
        {
            new User(
                UserId.NewId(),
                new Email("admin@solvestation.com"),
                "System",
                "Administrator",
                HashPasswordSafe("admin123"),
                UserRole.Admin
            ),
            new User(
                UserId.NewId(),
                new Email("dr.smith@hospital.com"),
                "John",
                "Smith",
                HashPasswordSafe("doctor123"),
                UserRole.Doctor
            ),
            new User(
                UserId.NewId(),
                new Email("pharmacist@pharmacy.com"),
                "Sarah",
                "Johnson",
                HashPasswordSafe("pharma123"),
                UserRole.Pharmacist
            ),
            new User(
                UserId.NewId(),
                new Email("patient@email.com"),
                "Michael",
                "Davis",
                HashPasswordSafe("patient123"),
                UserRole.Patient
            )
        };

        await context.Users.AddRangeAsync(users);
    }

    private static async Task SeedDrugsAsync(PharmacyDbContext context)
    {
        var drugs = new List<Drug>
        {
            new Drug(
                DrugId.NewId(),
                "Paracetamol 500mg",
                "Pain reliever and fever reducer",
                2.50m,
                1000,
                100,
                DateTime.UtcNow.AddYears(2),
                "PharmaCorp Ltd",
                "BATCH001",
                "PAR500"
            ),
            new Drug(
                DrugId.NewId(),
                "Amoxicillin 250mg",
                "Antibiotic for bacterial infections",
                5.75m,
                500,
                50,
                DateTime.UtcNow.AddYears(1),
                "MediSupply Inc",
                "BATCH002",
                "AMX250"
            ),
            new Drug(
                DrugId.NewId(),
                "Ibuprofen 400mg",
                "Anti-inflammatory pain reliever",
                3.25m,
                750,
                75,
                DateTime.UtcNow.AddMonths(18),
                "HealthMeds Co",
                "BATCH003",
                "IBU400"
            ),
            new Drug(
                DrugId.NewId(),
                "Aspirin 100mg",
                "Blood thinner and pain reliever",
                1.80m,
                25, // Low stock for testing alerts
                50,
                DateTime.UtcNow.AddDays(45), // Expiring soon for testing
                "CardioPharm",
                "BATCH004",
                "ASP100"
            ),
            new Drug(
                DrugId.NewId(),
                "Metformin 500mg",
                "Diabetes medication",
                4.50m,
                300,
                30,
                DateTime.UtcNow.AddYears(3),
                "DiabetesCare Ltd",
                "BATCH005",
                "MET500"
            )
        };

        await context.Drugs.AddRangeAsync(drugs);
    }
}
