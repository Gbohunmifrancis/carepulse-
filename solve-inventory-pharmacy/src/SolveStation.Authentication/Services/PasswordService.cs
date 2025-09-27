using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using SolveStation.Common.Interfaces;

namespace SolveStation.Authentication.Services;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        var salt = GenerateSalt();
        var hash = HashPasswordInternal(password, salt);
        return $"{Convert.ToBase64String(hash)}.{Convert.ToBase64String(salt)}";
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var parts = hashedPassword.Split('.');
        if (parts.Length != 2) return false;

        var storedHash = Convert.FromBase64String(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);

        var computedHash = HashPasswordInternal(password, salt);
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }

    private byte[] HashPasswordInternal(string password, byte[] salt)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = 8,
            MemorySize = 65536,
            Iterations = 4
        };
        return argon2.GetBytes(32);
    }

    private byte[] GenerateSalt()
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }
}
