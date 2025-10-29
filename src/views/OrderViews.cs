using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class OrderViews
{
public static string List(List<Order> orders, string username)
{
    var sb = new StringBuilder();
    sb.Append("<h2>Your Orders</h2>");
    sb.Append("<table border='1' cellpadding='6'><tr><th>ID</th><th>Total</th><th>Shipping</th><th>Arrival</th><th>Status</th><th>Date</th><th>Actions</th></tr>");

    foreach (var o in orders)
    {
        string adminBtn = "";
        if (o.Status == "Pending")
        {
            adminBtn = $"<a href='/orders/mark?id={o.Id}'>Mark Delivered</a>";
        }

        sb.Append($@"
            <tr>
                <td>{o.Id}</td>
                <td>${o.TotalPrice:F2}</td>
                <td>${o.Shipping:F2}</td>
                <td>{o.ArrivalDate:yyyy-MM-dd}</td>
                <td>{o.Status}</td>
                <td>{o.CreatedAt:yyyy-MM-dd HH:mm}</td>
                <td><a href='/orders/view?id={o.Id}'>View</a> {adminBtn}</td>
            </tr>");
    }

    sb.Append("</table>");
    return sb.ToString();
}


    public static string Details(List<OrderItem> items, int orderId)
    {
        var sb = new StringBuilder();
        sb.Append($"<h2>Order #{orderId}</h2>");
        sb.Append("<table border='1' cellpadding='6'><tr><th>Name</th><th>Quantity</th><th>Price</th></tr>");
        foreach (var i in items)
        {
            sb.Append($@"<tr><td>{i.Name}</td><td>{i.Quantity}</td><td>${i.Price:F2}</td></tr>");
        }
        sb.Append("</table>");
        sb.Append("<br><a href='/orders'>Back to orders</a>");
        return sb.ToString();
    }
}
