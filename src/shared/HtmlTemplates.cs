using System;

namespace SuperMarketDashboard;

public static class HtmlTemplates
{
    public static string Base(
        string title,
        string body,
        string message = "",
        string username = "",
        string role = "",
        bool includeNavbar = true)
    {
        // 🧭 Modern Navigation Bar
        string navbar = includeNavbar ? @$"
        <nav>
            <h1>SuperMarket Dashboard</h1>
            <ul>
                <li><a href='/'>Inventory</a></li>
                <li><a href='/warehouse'>Warehouse</a></li>
                <li><a href='/orders'>Orders</a></li>
                <li><a href='/users'>Users</a></li>
                <li><a href='/analytics'>Analytics</a></li>
                <li><a href='/logout'>Logout</a></li>
                {(string.IsNullOrEmpty(username) ? "" : $"<li class='user-info'>👤 {username} ({role})</li>")}
            </ul>
        </nav>" : "";

        // 🧩 Final Page Layout
        return $@"
        <!DOCTYPE html>
        <html lang='en'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>{title}</title>
            <link rel='stylesheet' href='/css/styles.css'>
        </head>
        <body>
            {navbar}
            <div class='container'>
                {(string.IsNullOrEmpty(message) ? "" : $"<div class='message'>{message}</div>")}
                {body}
            </div>
            <footer>SuperMarket Dashboard © 2025</footer>
        </body>
        </html>";
    }
}
