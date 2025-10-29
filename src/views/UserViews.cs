using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class UserViews
{
    public static string List(List<User> users, string role, string message)
    {
        var sb = new StringBuilder();
        sb.Append("<h2>Registered Users</h2>");

        if (!string.IsNullOrEmpty(message))
            sb.Append($"<p style='color:green'>{message}</p>");

        sb.Append("<table border='1' cellpadding='6'>");
        sb.Append("<tr><th>ID</th><th>Username</th><th>Role</th></tr>");

        foreach (var u in users)
        {
            sb.Append($@"
                <tr>
                    <td>{u.Id}</td>
                    <td>{u.Username}</td>
                    <td>{u.Role}</td>
                </tr>");
        }

        sb.Append("</table>");

        if (role == "admin")
            sb.Append("<p><a href='/users/register'>➕ Add New User</a></p>");

        return sb.ToString(); 
    }

    public static string Register(string message)
    {
        // Modern Add User Form — with proper double quotes
        string formHtml = $@"
            <div class=""auth-page"">
                <form method=""POST"" action=""/users/register"" class=""auth-form"">
                    <h2>👤 Add New User</h2>

                    {(string.IsNullOrEmpty(message) ? "" : $"<div class=\"message\">{message}</div>")}

                    <label for=""username"">Username</label>
                    <input id=""username"" name=""username"" type=""text"" placeholder=""Enter username"" required>

                    <label for=""password"">Password</label>
                    <input id=""password"" name=""password"" type=""password"" placeholder=""Enter password"" required>

                    <label for=""role"">Role</label>
                    <select id=""role"" name=""role"" required>
                        <option value=""user"">User</option>
                        <option value=""admin"">Admin</option>
                    </select>

                    <div class=""actions"">
                        <input type=""submit"" value=""Create User"">
                    </div>

                    <p class=""note""><a href=""/users"">← Back to Users</a></p>
                </form>
            </div>";

        return HtmlTemplates.Base("Register User", formHtml);
    }

}
