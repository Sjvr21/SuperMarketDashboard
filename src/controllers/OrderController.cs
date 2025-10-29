using System.Net;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SuperMarketDashboard;

public class OrderController
{
    private readonly OrderService service;
    private readonly MySqlWarehouseRepository warehouseRepo;

    private readonly Dictionary<string, List<WarehouseCartItem>> carts = new();

    public OrderController(OrderService service, MySqlWarehouseRepository warehouseRepo)
    {
        this.service = service;
        this.warehouseRepo = warehouseRepo; // ✅ initialize it

    }

    // Add item to user’s in-memory cart
    public async Task AddToCart(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        var form = (Hashtable)options["form"];
        int itemId = int.Parse(form["id"]?.ToString() ?? "0");
        int quantity = int.Parse(form["quantity"]?.ToString() ?? "1");

        if (!carts.ContainsKey(username))
            carts[username] = new List<WarehouseCartItem>();

        // ✅ Fetch item info from DB
        var item = await warehouseRepo.GetById(itemId);
        if (item == null)
        {
            res.Redirect("/warehouse?message=Item%20not%20found");
            res.Close();
            return;
        }

        // ✅ Merge duplicates (optional but recommended)
        var existing = carts[username].Find(c => c.ItemId == item.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            carts[username].Add(new WarehouseCartItem
            {
                ItemId = item.Id,
                Name = item.Name,
                Price = item.Price,
                Quantity = quantity
            });
        }

        res.Redirect("/warehouse?message=Item%20added%20to%20cart");
        res.Close();
    }



    // Confirm the order

    public async Task Confirm(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";

        try
        {
            // 🧩 Check if user has a cart
            if (!carts.ContainsKey(username) || carts[username].Count == 0)
            {
                res.Redirect("/cart?message=Cart%20is%20empty");
                await res.OutputStream.FlushAsync();
                res.Close();
                return;
            }

            var cart = carts[username];

            // 🧩 Create the order in DB
            await service.CreateOrder(username, cart);

            // 🧩 Clear the cart after successful creation
            carts[username].Clear();

            Console.WriteLine($"✅ Order placed successfully by {username}");

            // 🧩 Redirect properly to orders page
            res.StatusCode = (int)HttpStatusCode.Redirect;
            res.RedirectLocation = "/orders?message=Order%20placed%20successfully";

            // ✅ Ensure response headers are sent before closing
            await res.OutputStream.FlushAsync();
            res.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error confirming order: {ex.Message}");
            try
            {
                res.StatusCode = (int)HttpStatusCode.Redirect;
                res.RedirectLocation = "/cart?message=Error%20placing%20order";
                await res.OutputStream.FlushAsync();
                res.Close();
            }
            catch { /* ignore nested errors */ }
        }
    }





    // List all orders
    public async Task ViewAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        string role = options["role"]?.ToString() ?? "user";
        bool isAdmin = role == "admin";

        var orders = await service.GetOrders(username, isAdmin);
        string html = HtmlTemplates.Base("Orders", OrderViews.List(orders, username), "", username, role);

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }

    // View details of a single order
    public async Task ViewOrder(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int orderId = int.Parse(req.QueryString["id"] ?? "0");
        var items = await service.GetOrderItems(orderId);
        string html = HtmlTemplates.Base("Order Details", OrderViews.Details(items, orderId), "", options["username"]?.ToString() ?? "Guest", options["role"]?.ToString() ?? "user");

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }

    // Show the cart
    public async Task ViewCart(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        if (!carts.ContainsKey(username))
            carts[username] = new List<WarehouseCartItem>();

        var cart = carts[username];
        string html = HtmlTemplates.Base("Your Cart", CartViews.Show(cart, username), "", username, options["role"]?.ToString() ?? "user");

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }

    // Clear cart
    public async Task ClearCart(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        if (carts.ContainsKey(username))
            carts[username].Clear();

        res.Redirect("/cart?message=Cart%20cleared");
        res.Close();
    }

    public async Task MarkDelivered(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string role = options["role"]?.ToString() ?? "user";
        if (role != "admin")
        {
            res.Redirect("/orders?message=Not%20authorized");
            res.Close();
            return;
        }

        int orderId = int.Parse(req.QueryString["id"] ?? "0");

        try
        {
            // ✅ Mark the order as delivered
            await service.UpdateStatus(orderId, "Delivered");

            // ✅ Add items to inventory
            await service.RestockInventory(orderId);

            res.Redirect("/orders?message=Order%20marked%20as%20Delivered%20and%20restocked");
            res.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error marking delivered: {ex.Message}");
            res.Redirect("/orders?message=Error%20restocking%20inventory");
            res.Close();
        }
    }


    public async Task RemoveFromCart(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        try
        {
            string username = options["username"]?.ToString() ?? "Guest";
            var form = (Hashtable)options["form"];
            int id = int.Parse(form["id"]?.ToString() ?? "0");

            if (carts.ContainsKey(username))
            {
                carts[username].RemoveAll(i => i.ItemId == id);
            }

            Console.WriteLine($"🗑 {username} removed item {id} from cart.");

            res.Redirect("/cart?message=Item%20removed");
            res.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error removing item: {ex.Message}");
            res.Redirect("/cart?message=Error%20removing%20item");
            res.Close();
        }
    }

    private void SafeRedirect(HttpListenerResponse res, string url)
    {
        try
        {
            res.StatusCode = (int)HttpStatusCode.Redirect;
            res.RedirectLocation = url;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Redirect error: {ex.Message}");
        }
        finally
        {
            try
            {
                if (res.OutputStream.CanWrite)
                    res.OutputStream.Close();
            }
            catch { /* ignore double-close errors */ }
        }
    }







}
