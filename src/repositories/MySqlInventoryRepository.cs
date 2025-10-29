using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlInventoryRepository
{
    private readonly string connectionString;
    public string ConnectionString => connectionString;

    public MySqlInventoryRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    // =======================
    // Read all inventory items
    // =======================
    public async Task<List<InventoryItem>> ReadAll()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT id, name, category, price, quantity FROM inventory;";

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<InventoryItem>();

        int idIndex = reader.GetOrdinal("id");
        int nameIndex = reader.GetOrdinal("name");
        int categoryIndex = reader.GetOrdinal("category");
        int priceIndex = reader.GetOrdinal("price");
        int quantityIndex = reader.GetOrdinal("quantity");

        while (await reader.ReadAsync())
        {
            // ✅ Safely read using column indexes
            var item = new InventoryItem
            {
                Id = reader.GetInt32(idIndex),
                Name = reader.GetString(nameIndex),
                Category = reader.GetString(categoryIndex),
                Price = (decimal)reader.GetDouble(priceIndex),
                Quantity = reader.GetInt32(quantityIndex)
            };

            list.Add(item);
        }

        return list;
    }

    // =======================
    // Update item quantity
    // =======================
    public async Task UpdateQuantity(int id, int newQty)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "UPDATE inventory SET quantity = @qty WHERE id = @id;";
        cmd.Parameters.AddWithValue("@qty", newQty);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
