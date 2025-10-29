using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class MySqlOrderRepository
{
    private readonly string connectionString;

    public MySqlOrderRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    private MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    // ===========================
    // CREATE ORDER + ITEMS
    // ===========================
    public async Task<int> CreateOrder(Order order, List<OrderItem> items)
    {
        using var conn = new MySqlConnection(connectionString);
        await conn.OpenAsync();

        using var tx = await conn.BeginTransactionAsync();

        try
        {
            // 1️⃣ Insert into orders table
            using var cmdOrder = new MySqlCommand(@"
            INSERT INTO orders (username, total_price, shipping, arrival_date, status)
            VALUES (@username, @total_price, @shipping, @arrival_date, @status);
            SELECT LAST_INSERT_ID();", conn, (MySqlTransaction)tx);

            cmdOrder.Parameters.AddWithValue("@username", order.Username);
            cmdOrder.Parameters.AddWithValue("@total_price", order.TotalPrice);
            cmdOrder.Parameters.AddWithValue("@shipping", order.Shipping);
            cmdOrder.Parameters.AddWithValue("@arrival_date", order.ArrivalDate);
            cmdOrder.Parameters.AddWithValue("@status", order.Status);

            var orderId = Convert.ToInt32(await cmdOrder.ExecuteScalarAsync());

            // 2️⃣ Insert items — note parameter names now prefixed to avoid name collision
            foreach (var item in items)
            {
                using var cmdItem = new MySqlCommand(@"
                INSERT INTO `supermarketdb`.`order_items` 
                (`order_id`, `item_id`, `quantity`, `price`)
                VALUES (@p_order_id, @p_item_id, @p_quantity, @p_price);", conn, (MySqlTransaction)tx);

                cmdItem.Parameters.AddWithValue("@p_order_id", orderId);
                cmdItem.Parameters.AddWithValue("@p_item_id", item.ItemId);
                cmdItem.Parameters.AddWithValue("@p_quantity", item.Quantity);
                cmdItem.Parameters.AddWithValue("@p_price", item.Price);

                await cmdItem.ExecuteNonQueryAsync();
            }

            await tx.CommitAsync();
            return orderId;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            Console.WriteLine($"❌ SQL Error inserting order: {ex.Message}");
            throw;
        }
    }




    // ===========================
    // READ ALL ORDERS
    // ===========================
    public async Task<List<Order>> ReadAll(string username, bool isAdmin)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = isAdmin
            ? "SELECT * FROM orders ORDER BY id DESC"
            : "SELECT * FROM orders WHERE username=@u ORDER BY id DESC";
        if (!isAdmin)
            cmd.Parameters.AddWithValue("@u", username);

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<Order>();

        while (await reader.ReadAsync())
        {
            list.Add(new Order
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("total_price")),
                Shipping = reader.GetDecimal(reader.GetOrdinal("shipping")),
                ArrivalDate = reader.GetDateTime(reader.GetOrdinal("arrival_date")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
            });
        }

        return list;
    }

    // ===========================
    // READ ORDER ITEMS
    // ===========================
    public async Task<List<OrderItem>> ReadItems(int orderId)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT oi.id, oi.order_id, oi.item_id, oi.quantity, oi.price, wi.name
            FROM order_items oi
            JOIN warehouse_items wi ON oi.item_id = wi.id
            WHERE oi.order_id = @o";
        cmd.Parameters.AddWithValue("@o", orderId);

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<OrderItem>();

        while (await reader.ReadAsync())
        {
            list.Add(new OrderItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                OrderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                ItemId = reader.GetInt32(reader.GetOrdinal("item_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                Name = reader.GetString(reader.GetOrdinal("name"))
            });
        }

        return list;
    }

    // Reduce stock for each ordered item
    public async Task DecreaseStock(List<OrderItem> items)
    {
        using var dbc = OpenDb();
        foreach (var item in items)
        {
            using var cmd = dbc.CreateCommand();
            cmd.CommandText = "UPDATE warehouse_items SET quantity = quantity - @q WHERE id = @id";
            cmd.Parameters.AddWithValue("@q", item.Quantity);
            cmd.Parameters.AddWithValue("@id", item.ItemId);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task IncreaseInventory(List<OrderItem> items)
    {
        using var dbc = OpenDb();
        Console.WriteLine($"🔧 Restocking {items.Count} items into inventory...");

        foreach (var item in items)
        {
            Console.WriteLine($"📦 Restocking item: {item.Name} (Qty: {item.Quantity})");

            using var checkCmd = dbc.CreateCommand();
            checkCmd.CommandText = "SELECT id, quantity FROM inventory WHERE name = @name LIMIT 1";
            checkCmd.Parameters.AddWithValue("@name", item.Name ?? "Unknown");

            using var reader = await checkCmd.ExecuteReaderAsync();
            bool exists = await reader.ReadAsync();
            int existingId = 0;
            int existingQty = 0;

            if (exists)
            {
                existingId = reader.GetInt32(0);
                existingQty = reader.GetInt32(1);
            }
            await reader.CloseAsync();

            if (exists)
            {
                // ✅ Update quantity if item already exists
                using var updateCmd = dbc.CreateCommand();
                updateCmd.CommandText = "UPDATE inventory SET quantity = quantity + @quantity WHERE id = @id";
                updateCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                updateCmd.Parameters.AddWithValue("@id", existingId);
                await updateCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"🔄 Updated inventory for '{item.Name}' (+{item.Quantity})");
            }
            else
            {
                // ✅ Insert new item if not exists
                using var insertCmd = dbc.CreateCommand();
                insertCmd.CommandText = @"
                INSERT INTO inventory (name, category, quantity, price)
                VALUES (@name, 'General', @quantity, @price)";
                insertCmd.Parameters.AddWithValue("@name", item.Name ?? "Unknown");
                insertCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                insertCmd.Parameters.AddWithValue("@price", item.Price);
                await insertCmd.ExecuteNonQueryAsync();
                Console.WriteLine($"🆕 Added new item to inventory: '{item.Name}'");
            }
        }

        Console.WriteLine("✅ Inventory restock complete!");
    }




    // Update order status
    public async Task UpdateStatus(int orderId, string status)
    {
        using var dbc = OpenDb();
        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "UPDATE orders SET status=@s WHERE id=@id";
        cmd.Parameters.AddWithValue("@s", status);
        cmd.Parameters.AddWithValue("@id", orderId);
        await cmd.ExecuteNonQueryAsync();
    }

}
