using SolveStation.Data.Models;
using System.Security.Claims;

namespace SolveStation.Authentication.Services;

public interface ITokenService
{
    string GenerateJwtToken(User user);
    ClaimsPrincipal ValidateToken(string token);
    string GenerateRefreshToken();
    Task StoreRefreshTokenAsync(string refreshToken, UserId userId, DateTime expiry);
    Task<bool> IsRefreshTokenValidAsync(string token, UserId userId); 
    Task RevokeRefreshTokenAsync(string token);
}
