using SolveStation.Common.Models;

namespace SolveStation.Data.Models;

public class User : BaseEntity<UserId>
{
    public Email Email { get; private set; } = null!;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    private User() : base() { }

    public User(UserId id, Email email, string firstName, string lastName,
                string passwordHash, UserRole role) : base()
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        Role = role;
        IsEmailVerified = false;

        AddDomainEvent(new UserCreatedEvent(id, email, role));
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        MarkAsUpdated();

        AddDomainEvent(new UserProfileUpdatedEvent(Id, firstName, lastName));
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkAsUpdated();

        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        MarkAsUpdated();

        AddDomainEvent(new UserEmailVerifiedEvent(Id, Email));
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}
