namespace SolveStation.Authentication.Models;

public record LoginRequest(
    [Required] string Email,
    [Required] string Password);
