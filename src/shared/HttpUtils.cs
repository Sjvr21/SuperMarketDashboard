using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SuperMarketDashboard;

public static class HttpUtils
{
    // ================================
    // 📂 Read form data middleware
    // ================================
    public static async Task ReadRequestFormData(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        if (req.HttpMethod == "POST" && req.HasEntityBody)
        {
            using var body = req.InputStream;
            using var reader = new StreamReader(body, req.ContentEncoding);
            string rawData = await reader.ReadToEndAsync();
            var form = ParseFormData(rawData);
            options["form"] = form;
        }
    }


    // ================================
    // 🧩 Parse URL-encoded form data
    // ================================
    private static Hashtable ParseFormData(string raw)
    {
        var result = new Hashtable();
        var pairs = raw.Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var kv = pair.Split('=');
            if (kv.Length == 2)
                result[WebUtility.UrlDecode(kv[0])] = WebUtility.UrlDecode(kv[1]);
        }
        return result;
    }

    // ================================
    // 🍪 Get cookie by name
    // ================================
    public static string? GetCookie(HttpListenerRequest req, string name)
    {
        if (req.Cookies[name] != null)
            return req.Cookies[name].Value;
        return null;
    }

    // ================================
    // 💬 Simplified HTML Response
    // ================================
    public static async Task Respond(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int statusCode, string html)
    {
        res.StatusCode = statusCode;
        res.ContentType = "text/html";
        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.OutputStream.Close();
        res.Close();
    }

    // ================================
    // 🧱 Serve Static Files (CSS, JS)
    // ================================
    public static async Task ServeStaticFile(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string urlPath = req.Url!.AbsolutePath.TrimStart('/');
        string filePath = Path.Combine("static", urlPath);

        if (File.Exists(filePath))
        {
            string ext = Path.GetExtension(filePath).ToLower();
            string contentType = ext switch
            {
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "text/plain"
            };

            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            res.ContentType = contentType;
            res.ContentLength64 = fileData.Length;
            await res.OutputStream.WriteAsync(fileData, 0, fileData.Length);
            res.OutputStream.Close();
        }
    }


    // ✅ 404 (existing 1-parameter)
    public static async Task NotFound(HttpListenerResponse res)
    {
        res.StatusCode = (int)HttpStatusCode.NotFound;
        string html = "<h2>404 - Not Found</h2><p>The requested page could not be found.</p>";
        byte[] buffer = Encoding.UTF8.GetBytes(html);
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.OutputStream.Close();
        res.Close();
    }

    public static async Task NotFound(HttpListenerRequest req, HttpListenerResponse res)
    {
        await NotFound(res);
    }


}
