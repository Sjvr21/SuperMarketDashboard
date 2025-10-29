using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlWarehouseRepository
{
    private readonly string connectionString;

    public MySqlWarehouseRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<List<WarehouseItem>> ReadAll()
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM warehouse_items ORDER BY id ASC;";
        using var reader = await cmd.ExecuteReaderAsync();

        var items = new List<WarehouseItem>();
        int idIdx = reader.GetOrdinal("id");
        int nameIdx = reader.GetOrdinal("name");
        int catIdx = reader.GetOrdinal("category");
        int priceIdx = reader.GetOrdinal("price");
        int stockIdx = reader.GetOrdinal("stock");

        while (await reader.ReadAsync())
        {
            items.Add(new WarehouseItem
            {
                Id = reader.GetInt32(idIdx),
                Name = reader.GetString(nameIdx),
                Category = reader.GetString(catIdx),
                Price = reader.GetDecimal(priceIdx),
                Stock = reader.GetInt32(stockIdx)
            });
        }

        return items;
    }

    public async Task<WarehouseItem?> GetById(int id)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT id, name, category, price, stock FROM warehouse_items WHERE id=@id";
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new WarehouseItem
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Category = reader.GetString(2),
                Price = reader.GetDecimal(3),
                Stock = reader.GetInt32(4)
            };
        }
        return null;
    }

}
