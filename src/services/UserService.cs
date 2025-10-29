using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SuperMarketDashboard;

public class UserService
{
    private readonly MySqlUserRepository repo;

    public UserService(MySqlUserRepository repo)
    {
        this.repo = repo;
    }

    // ✅ Hash password using SHA256
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    // ✅ Authenticate and return JWT-like token
    public async Task<string?> Authenticate(string username, string password)
    {
        var user = await repo.GetUserByUsername(username);
        if (user == null)
        {
            return null;
        }

        if (!PasswordUtils.VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        Console.WriteLine($"[AUTH SUCCESS] {username} logged in as {user.Role}");
        return AuthUtils.GenerateToken(username, user.Role);
    }


    // ✅ Create new user
    public async Task CreateUser(string username, string password, string role)
    {
        string hash = HashPassword(password);
        await repo.Create(username, hash, role);
    }

    // ✅ Read all users
    public Task<List<User>> ReadAll() => repo.ReadAll();

    public async Task<(List<User>, int)> ReadPaged(int page, int pageSize)
{
    return await repo.ReadPaged(page, pageSize);
}

}
