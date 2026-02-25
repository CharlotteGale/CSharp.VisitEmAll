using System.Security.Cryptography;
using System.Text;

namespace VisitEmAll.Services;

public class PasswordHasher
{
    public string Hash(string plain)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    public bool Verify(string plain, string hashed)
    {
        return Hash(plain) == hashed;
    }
}
