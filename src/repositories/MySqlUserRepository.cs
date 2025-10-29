using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlUserRepository
{
    private readonly string connectionString;

    public MySqlUserRepository(string conn)
    {
        connectionString = conn;
    }

    private MySqlConnection OpenDb()
    {
        var conn = new MySqlConnection(connectionString);
        conn.Open();
        return conn;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, username, passwordhash, role FROM users WHERE username=@u;";
        cmd.Parameters.AddWithValue("@u", username);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash")),
                Role = reader.GetString(reader.GetOrdinal("role"))
            };
        }
        return null;
    }

    public async Task<List<User>> ReadAll()
    {
        var users = new List<User>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, username, role, passwordhash FROM users;";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Role = reader.GetString(reader.GetOrdinal("role")),
                PasswordHash = reader.GetString(reader.GetOrdinal("passwordhash"))
            });
        }
        return users;
    }

    public async Task Create(string username, string passwordHash, string role)
    {
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO users (username, passwordhash, role) VALUES (@u, @p, @r);";
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", passwordHash);
        cmd.Parameters.AddWithValue("@r", role);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<(List<User>, int)> ReadPaged(int page, int pageSize)
    {
        var users = new List<User>();
        int totalCount;

        using var conn = OpenDb();

        // 🧮 Get total count
        using (var countCmd = conn.CreateCommand())
        {
            countCmd.CommandText = "SELECT COUNT(*) FROM users;";
            totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
        }

        // 📄 Get paginated results
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, username, role FROM users LIMIT @offset, @limit;";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
        cmd.Parameters.AddWithValue("@limit", pageSize);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var user = new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Role = reader.GetString(reader.GetOrdinal("role"))
            };

            users.Add(user);
        }

        return (users, totalCount);
    }


}




