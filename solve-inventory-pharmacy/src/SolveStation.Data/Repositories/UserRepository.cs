using Microsoft.EntityFrameworkCore;
using SolveStation.Data.Context;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public class UserRepository : BaseRepository<User, UserId>, IUserRepository
{
    public UserRepository(PharmacyDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .FirstOrDefaultAsync(u => u.Email.Equals(email), cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .Where(u => u.Role == role)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsEmailUniqueAsync(Email email, UserId? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = GetActiveEntities().Where(u => u.Email.Equals(email));

        if (excludeId != null)
        {
            query = query.Where(u => !u.Id.Equals(excludeId));
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        return await GetActiveEntities()
            .Where(u => u.FirstName.ToLower().Contains(lowerSearchTerm) ||
                       u.LastName.ToLower().Contains(lowerSearchTerm) ||
                       (u.FirstName + " " + u.LastName).ToLower().Contains(lowerSearchTerm))
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAndPasswordAsync(Email email, string passwordHash, CancellationToken cancellationToken = default)
    {
        return await GetActiveEntities()
            .FirstOrDefaultAsync(u => u.Email.Equals(email) && u.PasswordHash == passwordHash, cancellationToken);
    }
}
