using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Text;

namespace SuperMarketDashboard;

public class InventoryController
{



    private readonly InventoryService service;

    public InventoryController(InventoryService service)
    {
        this.service = service;
    }

    // ===============================
    // 📄 Display all inventory items
    // ===============================
    public async Task ViewAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        string role = options["role"]?.ToString() ?? "user";

        var items = await service.ReadAll();

        string html = HtmlTemplates.Base("Inventory", InventoryViews.InventoryList(items, username), "", username, role);
        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }


    // ===================================
    // ✏️ Edit (decrease) item quantity
    // ===================================
    public async Task EditQuantity(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        try
        {
            var form = (Hashtable)options["form"];
            if (form == null)
            {
                res.Redirect("/?message=Invalid%20form");
                res.Close();
                return;
            }

            int id = int.Parse(form["id"]?.ToString() ?? "0");
            int newQty = int.Parse(form["quantity"]?.ToString() ?? "0");

            var allItems = await service.ReadAll();
            var item = allItems.Find(x => x.Id == id);

            if (item == null)
            {
                res.Redirect("/?message=Item%20not%20found");
                res.Close();
                return;
            }

            string username = options["username"]?.ToString() ?? "Unknown";

            // ✅ Prevent increasing quantity manually
            if (newQty > item.Quantity)
            {
                Console.WriteLine($"User '{username}' tried to increase quantity for {item.Name} ({item.Id}) from {item.Quantity} → {newQty}");
                res.Redirect("/?message=You%20cannot%20increase%20quantity%20manually");
                res.Close();
                return;
            }

            // ✅ Prevent negatives
            if (newQty < 0)
                newQty = 0;

            // ✅ Update and log the change
            await service.UpdateQuantity(id, newQty, username);

            Console.WriteLine($"[{username}] updated product ID {id} → quantity {newQty}");
            res.Redirect("/?message=Quantity%20updated");
            res.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error editing quantity: {ex.Message}");
            res.Redirect("/?message=Error%20updating%20item");
            res.Close();
        }
    }
}
