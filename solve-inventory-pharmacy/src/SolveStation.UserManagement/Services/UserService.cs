using Microsoft.Extensions.Logging;
using SolveStation.Common.Exceptions;
using SolveStation.Common.Interfaces;
using SolveStation.Data.Models;
using SolveStation.Data.Repositories;
using CommonInterfaces = SolveStation.Common.Interfaces;
using DataUser = SolveStation.Data.Models.User;
using UM = SolveStation.UserManagement.Models;

namespace SolveStation.UserManagement.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly CommonInterfaces.IUnitOfWork _unitOfWork;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        CommonInterfaces.IUnitOfWork unitOfWork,
        IPasswordService passwordService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task<UM.UserResponse> CreateUserAsync(UM.CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        _logger.LogInformation("Creating user with email {Email}", request.Email);

        // Validate input (basic)
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Email is required");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("First name is required");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Last name is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Password is required");
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Role is required");
        }

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new SolveStation.Common.Exceptions.ValidationException($"Invalid role: {request.Role}");
        }

        var email = new Email(request.Email);

        // Ensure email uniqueness
        bool isUnique = await _userRepository.IsEmailUniqueAsync(email, null, cancellationToken);
        if (!isUnique)
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Email already exists");
        }

        string hashedPassword = _passwordService.HashPassword(request.Password);
        var user = new DataUser(UserId.NewId(), email, request.FirstName, request.LastName, hashedPassword, role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created user {UserId}", user.Id);
        return ToResponse(user);
    }

    public async Task<UM.UserResponse> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        UserId userId = ParseUserId(id);
        DataUser? user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        return user is null ? throw new NotFoundException($"User with ID {id} not found") : ToResponse(user);
    }

    public async Task<List<UM.UserSummaryResponse>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<DataUser> users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(ToSummary).ToList();
    }

    public async Task<List<UM.UserSummaryResponse>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserRole>(role, true, out UserRole roleEnum))
        {
            throw new SolveStation.Common.Exceptions.ValidationException($"Invalid role: {role}");
        }

        IEnumerable<DataUser> users = await _userRepository.GetByRoleAsync(roleEnum, cancellationToken);
        return users.Select(ToSummary).ToList();
    }

    public async Task<List<UM.UserSummaryResponse>> SearchUsersByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return new List<UM.UserSummaryResponse>();
        }

        var users = await _userRepository.SearchByNameAsync(searchTerm, cancellationToken);
        return users.Select(ToSummary).ToList();
    }

    public async Task<UM.UserResponse> UpdateUserAsync(string id, UM.UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        UserId userId = ParseUserId(id);
        DataUser user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException($"User with ID {id} not found");

        user.UpdateProfile(request.FirstName, request.LastName);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated profile for user {UserId}", user.Id);
        return ToResponse(user);
    }

    public async Task<bool> ChangePasswordAsync(string id, UM.ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.CurrentPassword))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("Current password is required");
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new SolveStation.Common.Exceptions.ValidationException("New password is required");
        }

        UserId userId = ParseUserId(id);
        DataUser user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException($"User with ID {id} not found");

        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedException("Current password is incorrect");
        }

        string newHash = _passwordService.HashPassword(request.NewPassword);
        user.ChangePassword(newHash);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Changed password for user {UserId}", user.Id);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string id, CancellationToken cancellationToken = default)
    {
        UserId userId = ParseUserId(id);
        DataUser user = await _userRepository.GetByIdAsync(userId, cancellationToken) ?? throw new NotFoundException($"User with ID {id} not found");

        if (!user.IsEmailVerified)
        {
            user.VerifyEmail();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Verified email for user {UserId}", user.Id);
        }
        else
        {
            _logger.LogInformation("Email already verified for user {UserId}", user.Id);
        }

        return true;
    }

    public async Task<bool> UpdateLastLoginAsync(string id, CancellationToken cancellationToken = default)
    {
        UserId userId = ParseUserId(id);
        DataUser user = await _userRepository.GetByIdAsync(userId, cancellationToken) 
            ?? throw new NotFoundException($"User with ID {id} not found");

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated last login for user {UserId}", user.Id);
        return true;
    }

    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
    {
        UserId userId = ParseUserId(id);

        await _userRepository.DeleteAsync(userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted (soft) user {UserId}", userId);
        return true;
    }

    private static UserId ParseUserId(string id)
    {
        return !Guid.TryParse(id, out Guid guid)
            ? throw new SolveStation.Common.Exceptions.ValidationException($"Invalid user id: {id}")
            : new UserId(guid);
    }

    private static UM.UserResponse ToResponse(DataUser u) => new(
        Id: u.Id.Value.ToString(),
        Email: u.Email.Value,
        FirstName: u.FirstName,
        LastName: u.LastName,
        FullName: $"{u.FirstName} {u.LastName}",
        Role: u.Role.ToString(),
        IsEmailVerified: u.IsEmailVerified,
        LastLoginAt: u.LastLoginAt,
        IsActive: u.IsActive,
        CreatedAt: u.CreatedAt,
        UpdatedAt: u.UpdatedAt);

    private static UM.UserSummaryResponse ToSummary(DataUser u) => new(
        Id: u.Id.Value.ToString(),
        FullName: $"{u.FirstName} {u.LastName}",
        Email: u.Email.Value,
        Role: u.Role.ToString(),
        LastLoginAt: u.LastLoginAt,
        CreatedAt: u.CreatedAt);
}
