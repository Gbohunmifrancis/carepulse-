using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolveStation.PharmacyApi.Models.DTOs;
using SolveStation.UserManagement.Services;
using UM = SolveStation.UserManagement.Models;

namespace SolveStation.PharmacyApi.Controllers;

/// <summary>
/// User management controller for CRUD operations on users
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="role">Filter by role</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(PagedResult<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? role = null)
    {
        _logger.LogInformation("Getting users - Page: {Page}, PageSize: {PageSize}, Role: {Role}",
            page, pageSize, role);

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        List<UM.UserSummaryResponse> summaries;
        if (!string.IsNullOrWhiteSpace(role))
        {
            summaries = await _userService.GetUsersByRoleAsync(role);
        }
        else
        {
            summaries = await _userService.GetAllUsersAsync();
        }

        var totalCount = summaries.Count;
        var pageItems = summaries
            .OrderBy(s => s.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Fetch detailed info for the current page to include IsEmailVerified, etc.
        var detailedDtos = new List<UserDto>(pageItems.Count);
        foreach (var s in pageItems)
        {
            var full = await _userService.GetUserByIdAsync(s.Id);
            detailedDtos.Add(MapToUserDto(full));
        }

        var result = new PagedResult<UserDto>
        {
            Items = detailedDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(Guid id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        var user = await _userService.GetUserByIdAsync(id.ToString());
        return Ok(MapToUserDto(user));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        var svcRequest = new UM.CreateUserRequest
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            Role = request.Role
        };

        var created = await _userService.CreateUserAsync(svcRequest);
        var dto = MapToUserDto(created);
        return CreatedAtAction(nameof(GetUser), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Updated user data</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        var svcRequest = new UM.UpdateUserRequest
        {
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var updated = await _userService.UpdateUserAsync(id.ToString(), svcRequest);
        return Ok(MapToUserDto(updated));
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Password change data</param>
    /// <returns>Success confirmation</returns>
    [HttpPost("{id:guid}/change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        _logger.LogInformation("Password change request for user ID: {UserId}", id);

        var svcRequest = new UM.ChangePasswordRequest
        {
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };

        await _userService.ChangePasswordAsync(id.ToString(), svcRequest);
        return Ok(new { Message = "Password changed successfully" });
    }

    /// <summary>
    /// Deactivate user account
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success confirmation</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id)
    {
        _logger.LogInformation("Deactivating user with ID: {UserId}", id);

        var success = await _userService.DeleteUserAsync(id.ToString());
        if (!success)
            return NotFound();

        return Ok(new { Message = "User deactivated successfully" });
    }

    private static UserDto MapToUserDto(UM.UserResponse user)
    {
        return new UserDto
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            LastLoginAt = user.LastLoginAt,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
