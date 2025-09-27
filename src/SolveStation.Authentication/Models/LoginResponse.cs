namespace SolveStation.Authentication.Models;

public record LoginResponse(
    Guid UserId,
    string Email,
    string Role,
    string Token,
    string RefreshToken,
    DateTime RefreshTokenExpiry,
    DateTime Expiration);
