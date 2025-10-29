using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class CartViews
{
    public static string Show(List<WarehouseCartItem> cart, string username)
    {
        var sb = new StringBuilder();
        sb.Append($"<h2>{username}'s Cart</h2>");

        if (cart == null || cart.Count == 0)
        {
            sb.Append("<p>Your cart is empty. <a href='/warehouse'>Go back to warehouse</a></p>");
            return sb.ToString();
        }

        decimal total = 0;
        sb.Append("<table border='1' cellpadding='6'>");
        sb.Append("<tr><th>Name</th><th>Quantity</th><th>Price</th><th>Subtotal</th><th>Actions</th></tr>");

        foreach (var item in cart)
        {
            decimal subtotal = item.Price * item.Quantity;
            total += subtotal;

            sb.Append($@"
            <tr>
                <td>{item.Name}</td>
                <td>{item.Quantity}</td>
                <td>${item.Price:F2}</td>
                <td>${subtotal:F2}</td>
                <td>
                    <form method='POST' action='/cart/remove'>
                        <input type='hidden' name='id' value='{item.ItemId}'>
                        <button type='submit'>🗑 Remove</button>
                    </form>
                </td>
            </tr>");
        }

        decimal shipping = 10 + (total * 0.05m);
        decimal grandTotal = total + shipping;

        sb.Append("</table>");
        sb.Append($"<p><b>Subtotal:</b> ${total:F2}</p>");
        sb.Append($"<p><b>Shipping:</b> ${shipping:F2}</p>");
        sb.Append($"<p><b>Total:</b> ${grandTotal:F2}</p>");

        sb.Append(@"
       <form action=""/orders/confirm"" method=""POST"" onsubmit=""return confirm('Confirm your order?');"">
            <button type=""submit"">Submit Order</button>
        </form>

        <form method='POST' action='/cart/clear' style='margin-top:10px;'>
            <button type='submit'>Clear Cart</button>
        </form>
        <p><a href='/warehouse'>← Back to Warehouse</a></p>
    ");

        return sb.ToString();
    }

}
