using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class LogsViews
{
    public static string LogList(List<InventoryLog> logs, string username)
    {
        var sb = new StringBuilder();
        sb.Append("<h2>Inventory Change Logs</h2>");
        sb.Append("<table border='1' cellpadding='8' cellspacing='0' style='width:100%; border-collapse: collapse;'>");
        sb.Append("<tr style='background:#333;color:white;text-align:left;'>");
        sb.Append("<th>ID</th><th>User</th><th>Item ID</th><th>Old Qty</th><th>New Qty</th><th>Timestamp</th>");
        sb.Append("</tr>");

        foreach (var log in logs)
        {
            sb.Append($"<tr><td>{log.Id}</td><td>{log.Username}</td><td>{log.ItemId}</td><td>{log.OldQuantity}</td><td>{log.NewQuantity}</td><td>{log.Timestamp}</td></tr>");
        }

        sb.Append("</table>");

        if (logs.Count == 0)
        {
            sb.Append("<p style='margin-top:10px;'>No logs yet.</p>");
        }

        return sb.ToString();
    }
}
