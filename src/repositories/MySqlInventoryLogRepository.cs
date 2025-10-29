using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlInventoryLogRepository
{
    private readonly string connectionString;

    public MySqlInventoryLogRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    // Insert a log entry
    public async Task Create(string username, int itemId, int oldQty, int newQty)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO inventory_logs (username, item_id, old_quantity, new_quantity)
            VALUES (@u, @i, @old, @new);";
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@i", itemId);
        cmd.Parameters.AddWithValue("@old", oldQty);
        cmd.Parameters.AddWithValue("@new", newQty);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<InventoryLog>> ReadAll()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM inventory_logs ORDER BY timestamp DESC;";
        using var reader = await cmd.ExecuteReaderAsync();

        var logs = new List<InventoryLog>();

        // ✅ Get column indexes once
        int idIndex = reader.GetOrdinal("id");
        int usernameIndex = reader.GetOrdinal("username");
        int itemIdIndex = reader.GetOrdinal("item_id");
        int oldQtyIndex = reader.GetOrdinal("old_quantity");
        int newQtyIndex = reader.GetOrdinal("new_quantity");
        int timestampIndex = reader.GetOrdinal("timestamp");

        while (await reader.ReadAsync())
        {
            logs.Add(new InventoryLog
            {
                Id = reader.GetInt32(idIndex),
                Username = reader.GetString(usernameIndex),
                ItemId = reader.GetInt32(itemIdIndex),
                OldQuantity = reader.GetInt32(oldQtyIndex),
                NewQuantity = reader.GetInt32(newQtyIndex),
                Timestamp = reader.GetDateTime(timestampIndex)
            });
        }

        return logs;
    }

    public async Task CreateLog(string username, int itemId, int oldQty, int newQty)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"INSERT INTO inventory_logs (username, item_id, old_quantity, new_quantity, timestamp)
                        VALUES (@username, @item_id, @old_quantity, @new_quantity, NOW());";
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@item_id", itemId);
        cmd.Parameters.AddWithValue("@old_quantity", oldQty);
        cmd.Parameters.AddWithValue("@new_quantity", newQty);
        await cmd.ExecuteNonQueryAsync();
    }



}
