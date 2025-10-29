using System.Net;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class LogsController
{
    private readonly InventoryLogService service; // ✅ Add this field

    public LogsController(InventoryLogService service) // ✅ Inject via constructor
    {
        this.service = service;
    }

    public async Task ViewAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string username = options["username"]?.ToString() ?? "Guest";
        string role = options["role"]?.ToString() ?? "user";

        var logs = await service.ReadAll(); // ✅ Now it recognizes 'service'
        string html = HtmlTemplates.Base("Logs", LogsViews.LogList(logs, username), "", username, role);

        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }
}
