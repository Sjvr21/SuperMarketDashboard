using System.Net;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class WarehouseController
{
    private readonly WarehouseService service;

    public WarehouseController(WarehouseService service)
    {
        this.service = service;
    }

    public async Task ViewAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        string role = options["role"]?.ToString() ?? "user";

        var items = await service.ReadAll();
        string html = HtmlTemplates.Base("Warehouse", WarehouseViews.ItemList(items, username), "", username, role);

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }
}
