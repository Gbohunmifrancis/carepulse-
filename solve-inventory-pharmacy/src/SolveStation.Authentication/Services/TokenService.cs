using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SolveStation.Data.Models;
using SolveStation.Data.Repositories;

namespace SolveStation.Authentication.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IRefreshTokenRepository refreshTokenRepository)
    {
        _configuration = configuration;
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret is not configured"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim("role", user.Role.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Add both for compatibility
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret is not configured"));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = !string.IsNullOrEmpty(_configuration["JwtSettings:Issuer"]),
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidateAudience = !string.IsNullOrEmpty(_configuration["JwtSettings:Audience"]),
            ValidAudience = _configuration["JwtSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        RandomNumberGenerator.Fill(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> IsRefreshTokenValidAsync(string token, UserId userId)
    {
        var refreshToken = await _refreshTokenRepository.GetValidTokenAsync(token, userId);
        return refreshToken != null;
    }

    public async Task StoreRefreshTokenAsync(string token, UserId userId, DateTime expiry)
    {
        var refreshToken = new RefreshToken
        {
            Token = token,
            UserId = userId,
            ExpiryDate = expiry,
            CreatedDate = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
    }

    public async Task RevokeRefreshTokenAsync(string token)
    {
        await _refreshTokenRepository.RevokeAsync(token);
    }
}
