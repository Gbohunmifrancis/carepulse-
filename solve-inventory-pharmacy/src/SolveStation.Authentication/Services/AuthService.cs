using SolveStation.Authentication.Models;
using SolveStation.Data.Repositories;
using SolveStation.Data.Models;
using SolveStation.Common.Exceptions;
using SolveStation.Common.Interfaces;
using System.Security.Claims;

namespace SolveStation.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, string ipAddress)
    {
        var email = new Email(request.Email);
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Login failed for email: {email} from IP: {IpAddress}",
                request.Email, ipAddress);
            throw new AuthenticationException("Invalid credentials");
        }

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid password for user: {Email} from IP: {IpAddress}",
                request.Email, ipAddress);
            throw new AuthenticationException("Invalid credentials");
        }

        // Update last login
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        // Generate tokens
        var token = _tokenService.GenerateJwtToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Store refresh token
        await _tokenService.StoreRefreshTokenAsync(refreshToken, user.Id, refreshTokenExpiry);

        _logger.LogInformation("User {Email} logged in successfully from {IpAddress}",
            request.Email, ipAddress);

        return new LoginResponse(
            user.Id.Value,
            user.Email.Value,
            user.Role.ToString(),
            token,
            refreshToken,
            refreshTokenExpiry,
            DateTime.UtcNow.AddHours(1));
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        // Validate the refresh token
        var principal = _tokenService.ValidateToken(refreshToken);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        // Check if refresh token is valid in storage
        var userIdObj = new UserId(userId);
        if (!await _tokenService.IsRefreshTokenValidAsync(refreshToken, userIdObj))
        {
            throw new AuthenticationException("Invalid or expired refresh token");
        }

        var user = await _userRepository.FindByIdAsync(userIdObj);
        if (user == null || !user.IsActive)
        {
            throw new AuthenticationException("User not found or inactive");
        }

        var newToken = _tokenService.GenerateJwtToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Revoke old refresh token and store new one
        await _tokenService.RevokeRefreshTokenAsync(refreshToken);
        await _tokenService.StoreRefreshTokenAsync(newRefreshToken, user.Id, newRefreshTokenExpiry);

        _logger.LogInformation("Token refreshed for user {Email} from {IpAddress}",
            user.Email, ipAddress);

        return new LoginResponse(
            user.Id.Value,
            user.Email.Value,
            user.Role.ToString(),
            newToken,
            newRefreshToken,
            newRefreshTokenExpiry,
            DateTime.UtcNow.AddHours(1));
    }

    public async Task<bool> RevokeTokenAsync(string token, string ipAddress)
    {
        await _tokenService.RevokeRefreshTokenAsync(token);
        _logger.LogInformation("Token revoked from IP: {IpAddress}", ipAddress);
        return true;
    }
}


