using SolveStation.Common.Interfaces;
using SolveStation.Data.Models;

namespace SolveStation.Data.Repositories;

public interface IUserRepository : IRepository<User, UserId>
{
    // Explicit base repository methods for clarity
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User> AddAsync(User entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(User entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserId id, CancellationToken cancellationToken = default);
    
    // Specific user repository methods
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(Email email, UserId? excludeId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAndPasswordAsync(Email email, string passwordHash, CancellationToken cancellationToken = default);
}
