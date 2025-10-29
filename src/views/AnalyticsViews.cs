using System.Text;
using System.Collections.Generic;

namespace SuperMarketDashboard;

public static class AnalyticsViews
{
    public static string Dashboard(
        (decimal cost, int totalOrders, int deliveredOrders) summary,
        List<(string date, decimal cost)> daily,
        List<(string name, int quantity)> top)
    {
        var sb = new StringBuilder();
        sb.Append("<h2>📊 Purchase Analytics Dashboard</h2>");
        sb.Append("<h3>Summary</h3>");
        sb.Append($"<p><b>Total Spending:</b> ${summary.cost:F2}<br>");
        sb.Append($"<b>Total Orders Placed:</b> {summary.totalOrders}<br>");
        sb.Append($"<b>Delivered Orders:</b> {summary.deliveredOrders}</p>");

        sb.Append("<h3>💵 Daily Costs (Last 7 Days)</h3>");
        sb.Append("<table border='1' cellpadding='6'><tr><th>Date</th><th>Total Cost</th></tr>");
        foreach (var d in daily)
            sb.Append($"<tr><td>{d.date}</td><td>${d.cost:F2}</td></tr>");
        sb.Append("</table>");

        sb.Append("<h3>🥇 Best Ordered Products</h3>");
        sb.Append("<table border='1' cellpadding='6'><tr><th>Product</th><th>Quantity Ordered</th></tr>");
        foreach (var t in top)
            sb.Append($"<tr><td>{t.name}</td><td>{t.quantity}</td></tr>");
        sb.Append("</table>");

        return sb.ToString(); 
    }
}
