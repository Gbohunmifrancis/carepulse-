namespace SolveStation.Data.Models;

// User-related value objects and enums for the Data layer
public record UserId(Guid Value)
{
    public static UserId NewId() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);

    public override string ToString() => Value.ToString();
}

public record Email(string Value)
{
    private static string ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty");

        if (!email.Contains('@') || email.Length > 254)
            throw new ArgumentException("Invalid email format");

        return email.ToLowerInvariant();
    }
}

public enum UserRole
{
    Admin,
    Doctor,
    Pharmacist,
    Patient
}
