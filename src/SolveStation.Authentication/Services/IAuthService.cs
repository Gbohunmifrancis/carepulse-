using SolveStation.Authentication.Models;
namespace SolveStation.Authentication.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress);
    Task<LoginResponse> RefreshTokenAsync(string refreshToken, string ipAddress); 
    Task<bool> RevokeTokenAsync(string token, string ipAddress); 
}
