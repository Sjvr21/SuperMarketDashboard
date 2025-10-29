using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class AnalyticsService
{
    private readonly string connectionString;

    public AnalyticsService(string conn)
    {
        connectionString = conn;
    }

    private MySqlConnection OpenDb()
    {
        var conn = new MySqlConnection(connectionString);
        conn.Open();
        return conn;
    }


    public async Task<List<(string, double)>> GetBestOrderedProducts()
    {
        var result = new List<(string, double)>();
        using var dbc = new MySqlConnection(connectionString);
        await dbc.OpenAsync();

        string sql = @"
        SELECT i.name, SUM(o.total_price + o.shipping) AS total_spent
        FROM orders o
        JOIN inventory i ON o.id = i.id
        WHERE o.status = 'delivered'
        GROUP BY i.name
        ORDER BY total_spent DESC
        LIMIT 10;
    ";

        using var cmd = new MySqlCommand(sql, dbc);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            string productName = reader.GetString(0);
            double spent = reader.GetDouble(1);
            result.Add((productName, spent));
        }

        return result;
    }




    // 💸 Daily Costs
    public async Task<List<(string, double)>> GetDailyCosts()
    {
        var result = new List<(string, double)>();
        using var dbc = new MySqlConnection(connectionString);
        await dbc.OpenAsync();

        string sql = @"
            SELECT DATE(o.created_at) AS order_date, 
                SUM(o.total_price + o.shipping) AS total_cost
            FROM orders o
            WHERE o.status = 'delivered'
            GROUP BY DATE(o.created_at)
            ORDER BY DATE(o.created_at) DESC
            LIMIT 7;
        ";

        using var cmd = new MySqlCommand(sql, dbc);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            DateTime dateValue = reader.GetDateTime(0);
            string date = dateValue.ToString("yyyy-MM-dd");
            double total = reader.GetDouble(1);
            result.Add((date, total));
        }

        return result;
    }




    // 🧺 Most stocked products
    public async Task<List<(string Name, int Quantity)>> GetTopStockedProducts()
    {
        var list = new List<(string, int)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT name, quantity
            FROM inventory
            ORDER BY quantity DESC
            LIMIT 5;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            int qty = reader.GetInt32(reader.GetOrdinal("quantity"));
            list.Add((name, qty));
        }
        return list;
    }

    // 💸 Inventory value per category
    public async Task<List<(string Category, double TotalValue)>> GetValuePerCategory()
    {
        var list = new List<(string, double)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT category, SUM(quantity * price) AS total_value
            FROM inventory
            GROUP BY category
            ORDER BY total_value DESC;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string category = reader.GetString(reader.GetOrdinal("category"));
            double totalValue = reader.IsDBNull(reader.GetOrdinal("total_value"))
                ? 0
                : reader.GetDouble(reader.GetOrdinal("total_value"));
            list.Add((category, totalValue));
        }
        return list;
    }

    // 📆 Recently updated items
    public async Task<List<(string Name, string Date)>> GetRecentlyUpdatedItems()
    {
        var list = new List<(string, string)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT name, DATE_FORMAT(last_updated, '%Y-%m-%d %H:%i') AS updated
            FROM inventory
            ORDER BY last_updated DESC
            LIMIT 5;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            string updated = reader.GetString(reader.GetOrdinal("updated"));
            list.Add((name, updated));
        }
        return list;
    }

    // ⚠️ Low stock warning
    public async Task<List<(string Name, int Quantity)>> GetLowStockItems()
    {
        var list = new List<(string, int)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT name, quantity
            FROM inventory
            WHERE quantity < 10
            ORDER BY quantity ASC
            LIMIT 5;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string name = reader.GetString(reader.GetOrdinal("name"));
            int qty = reader.GetInt32(reader.GetOrdinal("quantity"));
            list.Add((name, qty));
        }
        return list;
    }
}
