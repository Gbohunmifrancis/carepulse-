using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SolveStation.Common.Interfaces;
using SolveStation.Data.Context;
using SolveStation.Data.Models;
using SolveStation.Data.Repositories;
using SolveStation.Data.Seed;

namespace SolveStation.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        // Register Entity Framework DbContext
        services.AddDbContext<PharmacyDbContext>(options =>

            options.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(PharmacyDbContext).Assembly.FullName))
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors());

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repositories
        services.AddScoped<IDrugRepository, DrugRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Note: BaseRepository is abstract and cannot be registered directly
        // Specific repositories inherit from BaseRepository and implement the interfaces

        return services;
    }

    public static async Task MigrateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PharmacyDbContext>();
        
        try
        {
            // Check if database exists
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                // Database doesn't exist, create it with migrations
                await context.Database.MigrateAsync();
                return;
            }

            // Database exists - check if migration history table exists
            var migrationHistoryExists = await context.Database.GetAppliedMigrationsAsync();
            
            if (!migrationHistoryExists.Any())
            {
                // Database exists but no migration history - this is likely your issue
                // Get all pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                
                if (pendingMigrations.Any())
                {
                    // Check if tables already exist by trying to query one
                    var tablesExist = await CheckIfTablesExist(context);
                    
                    if (tablesExist)
                    {
                        // Tables exist but no migration history - mark all migrations as applied
                        var allMigrations = context.Database.GetMigrations();
                        foreach (var migration in allMigrations)
                        {
                            // Add migration to history without executing it
                            await context.Database.ExecuteSqlRawAsync(
                                "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})",
                                migration, "8.0.0");
                        }
                    }
                    else
                    {
                        // No tables exist, run migrations normally
                        await context.Database.MigrateAsync();
                    }
                }
            }
            else
            {
                // Migration history exists, run normal migration
                await context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            // If migration history table doesn't exist, create it first
            if (ex.Message.Contains("Invalid object name '__EFMigrationsHistory'") || 
                ex.Message.Contains("object named 'Drugs'"))
            {
                await HandleExistingSchemaWithoutMigrationHistory(context);
            }
            else
            {
                throw;
            }
        }
    }

    private static async Task<bool> CheckIfTablesExist(PharmacyDbContext context)
    {
        try
        {
            // Try to check if key tables exist
            var tableCount = await context.Database.ExecuteSqlRawAsync(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('Drugs', 'Users', 'Prescriptions')");
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static async Task HandleExistingSchemaWithoutMigrationHistory(PharmacyDbContext context)
    {
        try
        {
            // Create migration history table if it doesn't exist
            await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END");

            // Get all migrations and mark them as applied
            var allMigrations = context.Database.GetMigrations();
            foreach (var migration in allMigrations)
            {
                // Check if migration is already recorded
                var exists = await context.Database.ExecuteSqlRawAsync(
                    "SELECT COUNT(*) FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0}", migration);
                
                if (exists == 0)
                {
                    await context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})",
                        migration, "8.0.0");
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to synchronize migration history. Consider dropping and recreating the database, or manually fix migration history.", ex);
        }
    }

    public static async Task SeedDatabase(this IServiceProvider serviceProvider, Func<string, string>? hashPassword = null)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PharmacyDbContext>();
        await SeedData.SeedAsync(context, hashPassword);
    }
}
