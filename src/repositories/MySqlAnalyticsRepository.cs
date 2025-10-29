using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlAnalyticsRepository
{
    private readonly string connectionString;
    public MySqlAnalyticsRepository(string conn) => connectionString = conn;

    private MySqlConnection OpenDb()
    {
        var conn = new MySqlConnection(connectionString);
        conn.Open();
        return conn;
    }

    // 💰 Total cost summary
    public async Task<(decimal totalCost, int totalOrders, int deliveredOrders)> GetSummary()
    {
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT 
                SUM(total) AS total_cost,
                COUNT(*) AS total_orders,
                SUM(CASE WHEN status='Delivered' THEN 1 ELSE 0 END) AS delivered
            FROM orders;";
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            decimal totalCost = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
            int total = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
            int delivered = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
            return (totalCost, total, delivered);
        }
        return (0, 0, 0);
    }

    // 📆 Daily purchasing cost (last 7 days)
    public async Task<List<(string date, decimal cost)>> GetDailyCosts()
    {
        var list = new List<(string, decimal)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT DATE(created_at) AS day, SUM(total) AS daily_cost
            FROM orders
            WHERE status='Delivered'
            GROUP BY DATE(created_at)
            ORDER BY day DESC
            LIMIT 7;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string day = reader.GetDateTime(0).ToString("yyyy-MM-dd");
            decimal cost = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
            list.Add((day, cost));
        }
        return list;
    }

    // 🥇 Most ordered (best-ordered) products
    public async Task<List<(string name, int quantity)>> GetTopOrderedProducts()
    {
        var list = new List<(string, int)>();
        using var conn = OpenDb();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT wi.name, SUM(oi.quantity) AS total
            FROM order_items oi
            JOIN warehouse_items wi ON wi.id = oi.item_id
            GROUP BY wi.name
            ORDER BY total DESC
            LIMIT 5;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string name = reader.GetString(0);
            int qty = reader.GetInt32(1);
            list.Add((name, qty));
        }
        return list;
    }
}
