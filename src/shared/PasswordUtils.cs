using System.Security.Cryptography;
using System.Text;

namespace SuperMarketDashboard;

public static class PasswordUtils
{
    public static bool VerifyPassword(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        string enteredHash = Convert.ToBase64String(bytes);
        return enteredHash == storedHash;
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
