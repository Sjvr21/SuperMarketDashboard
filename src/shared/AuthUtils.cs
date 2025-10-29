using System;
using System.Text;
using System.Security.Cryptography;

namespace SuperMarketDashboard;

public static class AuthUtils
{
    private static readonly byte[] secret = Encoding.UTF8.GetBytes("SuperMarketSecretKey");

    private static string ToBase64Url(byte[] data)
    {
        return Convert.ToBase64String(data)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] FromBase64Url(string input)
    {
        string padded = input.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2: padded += "=="; break;
            case 3: padded += "="; break;
        }
        return Convert.FromBase64String(padded);
    }

    private static string Sign(string data)
    {
        using var hmac = new HMACSHA256(secret);
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = hmac.ComputeHash(bytes);
        return ToBase64Url(hash);
    }

    public static string GenerateToken(string username, string role)
    {
        // Use ticks for unambiguous expiry representation
        long expiryTicks = DateTime.UtcNow.AddHours(2).Ticks;
        string data = $"{username}|{role}|{expiryTicks}";
        string sig = Sign(data);
        string full = $"{data}|{sig}";
        return ToBase64Url(Encoding.UTF8.GetBytes(full));
    }

    public static bool ValidateToken(string token, out string username, out string role)
    {
        username = "";
        role = "";

        try
        {
            string decoded = Encoding.UTF8.GetString(FromBase64Url(token));
            var parts = decoded.Split('|');
            if (parts.Length != 4)
                return false;

            string data = $"{parts[0]}|{parts[1]}|{parts[2]}";
            string sig = parts[3];
            string expected = Sign(data);

            if (!expected.Equals(sig))
            {
                Console.WriteLine($"Signature mismatch\nExpected: {expected}\nGot:      {sig}");
                return false;
            }

            username = parts[0];
            role = parts[1];
            long ticks = long.Parse(parts[2]);
            DateTime expiry = new DateTime(ticks, DateTimeKind.Utc);
            return expiry > DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Token validation error: " + ex.Message);
            return false;
        }
    }
}
