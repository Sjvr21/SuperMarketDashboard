using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Text;

namespace SuperMarketDashboard;

public class AuthController
{
    private readonly UserService userService;

    public AuthController(UserService userService)
    {
        this.userService = userService;
    }

    public async Task LoginGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        // ✅ Build login page without navbar
        string content = HtmlTemplates.Base("Login", AuthViews.Login(message), "", "", "", includeNavbar: false);

        byte[] buffer = Encoding.UTF8.GetBytes(content);
        res.StatusCode = (int)HttpStatusCode.OK;
        res.ContentType = "text/html";
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        res.Close();
    }


    public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var form = (Hashtable)options["form"];
        string username = form["username"]?.ToString() ?? "";
        string password = form["password"]?.ToString() ?? "";

        string? token = await userService.Authenticate(username, password);
        if (token == null)
        {
            res.Redirect("/login?message=Invalid%20credentials");
            res.Close();
            return;
        }

        // ✅ Define the cookie correctly
        var cookie = new Cookie("token", token, "/");
        cookie.HttpOnly = false; // allow you to see it in browser for debugging
        cookie.Secure = false;   // set to true if using HTTPS

        res.AppendCookie(cookie);
        res.Redirect("/");
        res.Close();
    }

    public async Task Logout(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var cookie = new Cookie("token", "", "/");
        cookie.Expires = DateTime.Now.AddDays(-1); // delete cookie immediately
        res.AppendCookie(cookie);
        res.Redirect("/login?message=Logged%20out");
        res.Close();
    }
}
