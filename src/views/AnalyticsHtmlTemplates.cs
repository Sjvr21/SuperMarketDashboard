using System.Text;
using System.Collections.Generic;

namespace SuperMarketDashboard;

public static class AnalyticsHtmlTemplates
{
    public static string Dashboard(
        List<(string, int)> topStocked,
        List<(string, double)> categoryValues,
        List<(string, string)> recentUpdates,
        List<(string, int)> lowStock,
        List<(string, double)> bestOrdered,
        List<(string, double)> dailyCosts)
    {
        var sb = new StringBuilder();
        sb.Append(@"<link rel='stylesheet' href='/css/analytics.css'>");
        sb.Append("<div class='dashboard'>");
        sb.Append("<h1>📊 Analytics Dashboard</h1>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>🛍️ Best Ordered Products</h2>");
        sb.Append("<table><tr><th>Product</th><th>Total Spent ($)</th></tr>");
        foreach (var (name, total) in bestOrdered)
            sb.Append($"<tr><td>{name}</td><td>{total:F2}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>💸 Daily Costs</h2>");
        sb.Append("<table><tr><th>Date</th><th>Total Cost ($)</th></tr>");
        foreach (var (date, total) in dailyCosts)
            sb.Append($"<tr><td>{date}</td><td>{total:F2}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>📦 Top Stocked Products</h2>");
        sb.Append("<table><tr><th>Product</th><th>Quantity</th></tr>");
        foreach (var (name, qty) in topStocked)
            sb.Append($"<tr><td>{name}</td><td>{qty}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>💰 Value Per Category</h2>");
        sb.Append("<table><tr><th>Category</th><th>Total Value ($)</th></tr>");
        foreach (var (cat, val) in categoryValues)
            sb.Append($"<tr><td>{cat}</td><td>{val:F2}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>📆 Recently Updated</h2>");
        sb.Append("<table><tr><th>Product</th><th>Last Updated</th></tr>");
        foreach (var (name, date) in recentUpdates)
            sb.Append($"<tr><td>{name}</td><td>{date}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<div class='section-card'>");
        sb.Append("<h2>⚠️ Low Stock</h2>");
        sb.Append("<table><tr><th>Product</th><th>Quantity</th></tr>");
        foreach (var (name, qty) in lowStock)
            sb.Append($"<tr><td>{name}</td><td style='color:red;'>{qty}</td></tr>");
        sb.Append("</table></div>");

        sb.Append("<footer>SuperMarket Dashboard © 2025</footer>");
        sb.Append("</div>");
        return sb.ToString();
    }
}
