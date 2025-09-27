using SolveStation.Data.Models;
namespace SolveStation.Data.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetValidTokenAsync(string token, UserId userId);
    Task AddAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAsync(string token);
    Task CleanExpiredTokensAsync();
}
