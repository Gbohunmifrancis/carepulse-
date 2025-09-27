namespace SolveStation.Authentication.Models;

public record RefreshTokenRequest(
    [Required] string Token);
