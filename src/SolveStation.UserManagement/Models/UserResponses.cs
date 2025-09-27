using System.Collections.Generic;

namespace SolveStation.UserManagement.Models;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Role,
    bool IsEmailVerified,
    System.DateTime? LastLoginAt,
    bool IsActive,
    System.DateTime CreatedAt,
    System.DateTime UpdatedAt);

public record UserSummaryResponse(
    string Id,
    string FullName,
    string Email,
    string Role,
    System.DateTime? LastLoginAt,
    System.DateTime CreatedAt);

