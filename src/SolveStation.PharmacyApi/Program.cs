using SolveStation.Data.Extensions;
using SolveStation.PharmacyApi.Configuration;
using SolveStation.PharmacyApi.Middleware;
using Serilog;
using System.IdentityModel.Tokens.Jwt;

// Clear default claims mapping to prevent interference with custom claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Program).Assembly) // Main API controllers
    .AddApplicationPart(typeof(SolveStation.PrescriptionManagement.Controllers.PrescriptionsController).Assembly); // PrescriptionManagement controllers
builder.Services.AddEndpointsApiExplorer();

// Configure database and repositories
builder.Services.AddDataLayer(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Configure authentication and authorization
builder.Services.AddAuthenticationServices(builder.Configuration);

// Configure application services
builder.Services.AddApplicationServices();

// Configure API documentation
builder.Services.AddApiDocumentation();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PharmacyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3001") // Add your frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pharmacy System API v1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at root
    });
}
else
{
    // Only use HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseCors("PharmacyPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Custom middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

// Ensure database is created and seeded
try
{
    Log.Information("Ensuring database is created and seeded");
    await app.Services.MigrateDatabase();
    Log.Information("Database migration completed successfully");
    
    // Get password service for seeding
    using var scope = app.Services.CreateScope();
    var passwordService = scope.ServiceProvider.GetRequiredService<SolveStation.Common.Interfaces.IPasswordService>();
    
    await app.Services.SeedDatabase(passwordService.HashPassword);
    Log.Information("Database initialization completed");
}
catch (Microsoft.Data.SqlClient.SqlException sqlEx) when (sqlEx.Number == 2714)
{
    // Handle "object already exists" error specifically
    Log.Warning("Database schema already exists. Attempting to sync migration history...");
    try
    {
        // Try to run migration again - our improved logic should handle this
        await app.Services.MigrateDatabase();
        Log.Information("Migration history synchronized successfully");
        
        // Continue with seeding
        using var scope = app.Services.CreateScope();
        var passwordService = scope.ServiceProvider.GetRequiredService<SolveStation.Common.Interfaces.IPasswordService>();
        await app.Services.SeedDatabase(passwordService.HashPassword);
        Log.Information("Database initialization completed after sync");
    }
    catch (Exception retryEx)
    {
        Log.Error(retryEx, "Failed to sync migration history. Please run the migration script manually.");
        Log.Error("To fix this issue:");
        Log.Error("1. Run the PowerShell script: scripts/fix-migration-history.ps1");
        Log.Error("2. Or manually drop and recreate the database");
        Log.Error("3. Or use: dotnet ef database drop --force && dotnet ef database update");
        throw;
    }
}
catch (Exception ex)
{
    Log.Error(ex, "Error during database initialization");
    Log.Error("If this is a migration issue, try running: scripts/fix-migration-history.ps1");
    throw;
}

try
{
    Log.Information("Starting Pharmacy System API");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class accessible to integration tests
public partial class Program { }
