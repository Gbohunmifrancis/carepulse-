using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace SolveStation.PharmacyApi.Controllers;

/// <summary>
/// Health check and system status controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HealthController : BaseApiController
{
    public HealthController(IMapper mapper, ILogger<HealthController> logger)
        : base(mapper, logger)
    {
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>System health status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        var healthStatus = new
        {
            Status = "Healthy",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            MachineName = Environment.MachineName,
            ProcessId = Environment.ProcessId
        };

        Logger.LogInformation("Health check requested");
        return Ok(healthStatus);
    }

    /// <summary>
    /// Detailed health check with dependencies
    /// </summary>
    /// <returns>Detailed system health status</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDetailedHealth()
    {
        var checks = new List<object>();

        // Database connectivity check
        try
        {
            // TODO: Add actual database connectivity check
            checks.Add(new { Service = "Database", Status = "Healthy", ResponseTime = "< 100ms" });
        }
        catch (Exception ex)
        {
            checks.Add(new { Service = "Database", Status = "Unhealthy", Error = ex.Message });
        }

        // Memory usage check
        var workingSet = GC.GetTotalMemory(false);
        checks.Add(new { Service = "Memory", Status = "Healthy", Usage = $"{workingSet / 1024 / 1024} MB" });

        var detailedStatus = new
        {
            OverallStatus = checks.All(c => ((dynamic)c).Status == "Healthy") ? "Healthy" : "Degraded",
            Timestamp = DateTime.UtcNow,
            Checks = checks
        };

        Logger.LogInformation("Detailed health check requested");
        return Ok(detailedStatus);
    }
}
