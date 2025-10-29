using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Text;

namespace SuperMarketDashboard;

public class HomeController
{
    public async Task Index(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string content = HtmlTemplates.Base("Supermarket Dashboard", "<h1>Welcome to Supermarket Dashboard</h1>");
        byte[] buffer = Encoding.UTF8.GetBytes(content);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);


    }
}
