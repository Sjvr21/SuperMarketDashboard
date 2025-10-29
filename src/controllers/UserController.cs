using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class UserController
{
    private readonly UserService userService;

    public UserController(UserService userService)
    {
        this.userService = userService;
    }

    // ✅ USERS LIST (with pagination)
    public async Task ViewAll(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        // 🔒 Auth check
        string token = HttpUtils.GetCookie(req, "token");
        if (!AuthUtils.ValidateToken(token, out string username, out string role))
        {
            res.Redirect("/login?message=Please%20log%20in");
            res.Close();
            return;
        }

        // Pagination parameters
        int page = int.TryParse(req.QueryString["page"], out var p) ? p : 1;
        int pageSize = 10;

        // New paged method from repository/service
        var (users, totalCount) = await userService.ReadPaged(page, pageSize);
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        // Generate pagination HTML
        string pagination = PaginationUtils.RenderPagination("/users", page, totalPages);

        // Render user list view + pagination
        string html = UserViews.List(users, role, message)
            .Replace("</table>", "</table>" + pagination);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK,
            HtmlTemplates.Base("User List", html, message, username, role));
    }

    // ✅ REGISTER FORM (Admin only)
    public async Task RegisterGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        if (!AuthUtils.ValidateToken(HttpUtils.GetCookie(req, "token"), out string username, out string role) || role != "admin")
        {
            res.Redirect("/?message=Unauthorized");
            res.Close();
            return;
        }

        string html = UserViews.Register("");
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }

    // ✅ REGISTER POST (Admin only)
    public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        if (!AuthUtils.ValidateToken(HttpUtils.GetCookie(req, "token"), out string username, out string role) || role != "admin")
        {
            res.Redirect("/?message=Unauthorized");
            res.Close();
            return;
        }

        var form = (Hashtable)options["form"];
        string newUser = form["username"]?.ToString() ?? "";
        string password = form["password"]?.ToString() ?? "";
        string newRole = form["role"]?.ToString() ?? "user";

        if (string.IsNullOrWhiteSpace(newUser) || string.IsNullOrWhiteSpace(password))
        {
            res.Redirect("/users/register?message=Missing%20fields");
            res.Close();
            return;
        }

        await userService.CreateUser(newUser, password, newRole);
        res.Redirect("/users?message=User%20created%20successfully");
        res.Close();
    }
}
