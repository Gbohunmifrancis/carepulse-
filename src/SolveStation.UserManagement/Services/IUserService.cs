using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UM = SolveStation.UserManagement.Models;

namespace SolveStation.UserManagement.Services;

public interface IUserService
{
    Task<UM.UserResponse> CreateUserAsync(UM.CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<UM.UserResponse> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<List<UM.UserSummaryResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<List<UM.UserSummaryResponse>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default);

    Task<List<UM.UserSummaryResponse>> SearchUsersByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<UM.UserResponse> UpdateUserAsync(string id, UM.UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task<bool> ChangePasswordAsync(string id, UM.ChangePasswordRequest request, CancellationToken cancellationToken = default);

    Task<bool> VerifyEmailAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> UpdateLastLoginAsync(string id, CancellationToken cancellationToken = default);

    Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);
}
