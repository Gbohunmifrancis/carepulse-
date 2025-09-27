namespace SolveStation.Data.Models;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public UserId UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }
    public DateTime? RevokedDate { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
