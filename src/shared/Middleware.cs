using System.Net;
using System.Collections;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public static class Middleware
{
    public static async Task CheckAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var cookie = req.Cookies["token"];

        // 🔎 DEBUG LOGGING
        Console.WriteLine("---- AUTH DEBUG ----");
        Console.WriteLine("Cookie present? " + (cookie != null));
        Console.WriteLine("Cookie value: " + cookie?.Value);

        if (cookie == null)
        {
            Console.WriteLine("=> no cookie");
            res.Redirect("/login?message=Please%20log%20in");
            res.Close();
            return;
        }

        bool valid = AuthUtils.ValidateToken(cookie.Value, out string username, out string role);
        Console.WriteLine("ValidateToken() => " + valid);
        Console.WriteLine($"username={username}, role={role}");
        Console.WriteLine("--------------------");

        if (!valid)
        {
            res.Redirect("/login?message=Please%20log%20in");
            res.Close();
            return;
        }

        options["username"] = username;
        options["role"] = role;
    }

    public static async Task CheckAdmin(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        await CheckAuth(req, res, options);
        if (!options.ContainsKey("role") || (string)options["role"] != "admin")
        {
            res.Redirect("/?message=Access%20denied");
            res.Close();
        }
    }
}
