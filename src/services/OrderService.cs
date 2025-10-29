using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class OrderService
{
    private readonly MySqlOrderRepository repo;

    public OrderService(MySqlOrderRepository repo)
    {
        this.repo = repo;
    }

    // ==============================
    // CREATE ORDER
    // ==============================
    public async Task<int> CreateOrder(string username, List<WarehouseCartItem> cart)
    {
        if (cart.Count == 0)
            throw new Exception("Cart is empty");

        decimal total = 0;
        foreach (var c in cart)
            total += c.Price * c.Quantity;

        decimal shipping = 10 + (total * 0.05m);
        var order = new Order
        {
            Username = username,
            TotalPrice = total,
            Shipping = shipping,
            ArrivalDate = DateTime.UtcNow.AddDays(2),
            Status = "Pending"
        };

        var orderItems = cart.ConvertAll(c => new OrderItem
        {
            ItemId = c.ItemId,
            Quantity = c.Quantity,
            Price = c.Price
        });

        try
        {
            int orderId = await repo.CreateOrder(order, orderItems);
            await repo.DecreaseStock(orderItems);
            return orderId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Non-fatal SQL warning: {ex.Message}");
            return -1; // or handle gracefully
        }
    }

    // ==============================
    // READ ORDERS
    // ==============================
    public async Task<List<Order>> GetOrders(string username, bool isAdmin)
        => await repo.ReadAll(username, isAdmin);

    public async Task<List<OrderItem>> GetOrderItems(int orderId)
        => await repo.ReadItems(orderId);

    // ==============================
    // UPDATE ORDER STATUS
    // ==============================
    public async Task UpdateStatus(int orderId, string status)
        => await repo.UpdateStatus(orderId, status);

    public async Task RestockInventory(int orderId)
    {
        var items = await repo.ReadItems(orderId);
        await repo.IncreaseInventory(items);
    }
}




