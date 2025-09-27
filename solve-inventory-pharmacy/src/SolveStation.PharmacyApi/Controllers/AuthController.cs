using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveStation.PharmacyApi.Models.DTOs;
using SolveStation.Authentication.Services;
using SolveStation.Authentication.Models;
using SolveStation.UserManagement.Services;
using System.Security.Claims;

namespace SolveStation.PharmacyApi.Controllers;

/// <summary>
/// Authentication controller for user login and token management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService, IUserService userService)
    {
        _logger = logger;
        _authService = authService;
        _userService = userService;
    }

    /// <summary>
    /// Authenticate user and return JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Authentication token and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(SolveStation.PharmacyApi.Models.DTOs.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] SolveStation.PharmacyApi.Models.DTOs.LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var authRequest = new SolveStation.Authentication.Models.LoginRequest(request.Email, request.Password);
            var authResponse = await _authService.LoginAsync(authRequest, ipAddress);

            var response = new SolveStation.PharmacyApi.Models.DTOs.LoginResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresAt = authResponse.Expiration,
                User = new UserDto
                {
                    Id = authResponse.UserId,
                    Email = authResponse.Email,
                    Role = authResponse.Role,
                    CreatedAt = DateTime.UtcNow, // This would need to come from the user object
                    FirstName = string.Empty, // These would need to come from the user object
                    LastName = string.Empty,
                    IsEmailVerified = true,
                    LastLoginAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }
        catch (SolveStation.Common.Exceptions.AuthenticationException ex)
        {
            _logger.LogWarning("Authentication failed for {Email}: {Message}", request.Email, ex.Message);
            return Unauthorized(new { message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return BadRequest(new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <returns>New authentication token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(SolveStation.PharmacyApi.Models.DTOs.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] SolveStation.PharmacyApi.Models.DTOs.RefreshTokenRequest request)
    {
        _logger.LogInformation("Token refresh attempt");

        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var authResponse = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);

            var response = new SolveStation.PharmacyApi.Models.DTOs.LoginResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken,
                ExpiresAt = authResponse.Expiration,
                User = new UserDto
                {
                    Id = authResponse.UserId,
                    Email = authResponse.Email,
                    Role = authResponse.Role,
                    CreatedAt = DateTime.UtcNow,
                    FirstName = string.Empty,
                    LastName = string.Empty,
                    IsEmailVerified = true,
                    LastLoginAt = DateTime.UtcNow
                }
            };

            return Ok(response);
        }
        catch (SolveStation.Common.Exceptions.AuthenticationException ex)
        {
            _logger.LogWarning("Token refresh failed: {Message}", ex.Message);
            return Unauthorized(new { message = "Invalid refresh token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return BadRequest(new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Logout user and invalidate token
    /// </summary>
    /// <returns>Success confirmation</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Logout attempt for user");

        try
        {
            // Get the token from the authorization header
            var token = HttpContext.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                await _authService.RevokeTokenAsync(token, ipAddress);
            }

            return Ok(new { message = "Successfully logged out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Ok(new { message = "Successfully logged out" }); // Still return success for logout
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(SolveStation.PharmacyApi.Models.DTOs.UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        _logger.LogInformation("Get current user request");

        try
        {
            // Get user ID from the JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var userResponse = await _userService.GetUserByIdAsync(userIdClaim);
            
            var userDto = new UserDto
            {
                Id = Guid.Parse(userResponse.Id),
                Email = userResponse.Email,
                FirstName = userResponse.FirstName,
                LastName = userResponse.LastName,
                Role = userResponse.Role,
                CreatedAt = userResponse.CreatedAt,
                UpdatedAt = userResponse.UpdatedAt,
                LastLoginAt = userResponse.LastLoginAt,
                IsEmailVerified = userResponse.IsEmailVerified
            };

            return Ok(userDto);
        }
        catch (SolveStation.Common.Exceptions.NotFoundException ex)
        {
            _logger.LogWarning("User not found: {Message}", ex.Message);
            return Unauthorized(new { message = "User not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return BadRequest(new { message = "An error occurred while getting user information" });
        }
    }
}
