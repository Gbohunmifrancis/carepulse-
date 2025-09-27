using SolveStation.Data.Models;
using SolveStation.Data.Context;
namespace SolveStation.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PharmacyDbContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(PharmacyDbContext context, ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token, UserId userId)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt =>
                rt.Token == token &&
                rt.UserId == userId &&
                !rt.IsRevoked &&
                rt.ExpiryDate > DateTime.UtcNow);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        _logger.LogDebug("Refresh token stored for user {UserId}", refreshToken.UserId);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedDate = DateTime.UtcNow;

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Refresh token revoked: {Token}", token);
        }
    }

    public async Task CleanExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiryDate < DateTime.UtcNow || rt.IsRevoked)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Cleaned {Count} expired refresh tokens", expiredTokens.Count);
        }
    }
}
