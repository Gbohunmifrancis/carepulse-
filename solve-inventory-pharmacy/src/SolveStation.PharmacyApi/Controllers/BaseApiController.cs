using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Security.Claims;

namespace SolveStation.PharmacyApi.Controllers;

/// <summary>
/// Base controller with common functionality for all API controllers
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IMapper Mapper;
    protected readonly ILogger Logger;

    protected BaseApiController(IMapper mapper, ILogger logger)
    {
        Mapper = mapper;
        Logger = logger;
    }

    /// <summary>
    /// Get the current user ID from the JWT token claims
    /// </summary>
    /// <returns>Current user ID</returns>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }

    /// <summary>
    /// Get the current user role from the JWT token claims
    /// </summary>
    /// <returns>Current user role</returns>
    protected string GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst("role");
        return roleClaim?.Value ?? throw new UnauthorizedAccessException("User role not found in token");
    }

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    /// <param name="role">Role to check</param>
    /// <returns>True if user has the role</returns>
    protected bool HasRole(string role)
    {
        // First try the standard IsInRole method
        if (User.IsInRole(role))
            return true;
            
        // Then check the custom role claim directly
        var roleClaim = User.FindFirst("role");
        if (roleClaim != null && roleClaim.Value.Equals(role, StringComparison.OrdinalIgnoreCase))
            return true;
            
        // Also check ClaimTypes.Role for compatibility
        var standardRoleClaim = User.FindFirst(ClaimTypes.Role);
        if (standardRoleClaim != null && standardRoleClaim.Value.Equals(role, StringComparison.OrdinalIgnoreCase))
            return true;
            
        return false;
    }

    /// <summary>
    /// Create a standardized error response
    /// </summary>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>Error response</returns>
    protected IActionResult ErrorResponse(string message, int statusCode = 400)
    {
        var errorResponse = new
        {
            Message = message,
            StatusCode = statusCode,
            Timestamp = DateTime.UtcNow
        };

        return StatusCode(statusCode, errorResponse);
    }

    /// <summary>
    /// Create a standardized success response with data
    /// </summary>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Success response</returns>
    protected IActionResult SuccessResponse(object data, string message = "Success")
    {
        var response = new
        {
            Message = message,
            Data = data,
            Timestamp = DateTime.UtcNow
        };

        return Ok(response);
    }
}
