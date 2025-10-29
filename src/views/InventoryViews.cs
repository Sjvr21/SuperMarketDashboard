using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class InventoryViews
{
    public static string InventoryList(List<InventoryItem> items, string username)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<h2>Store Inventory</h2>");
        sb.Append($"<p>Signed in as: <b>{username}</b></p>");
        sb.Append("<table border='1' cellpadding='6'><tr><th>ID</th><th>Name</th><th>Category</th><th>Price</th><th>Quantity</th><th>Actions</th></tr>");

        foreach (var i in items)
        {
            sb.Append($@"
                <tr>
                    <td>{i.Id}</td>
                    <td>{i.Name}</td>
                    <td>{i.Category}</td>
                    <td>${i.Price:F2}</td>
                    <td>{i.Quantity}</td>
                    <td>
                        <form method='POST' action='/inventory/edit'>
                            <input type='hidden' name='id' value='{i.Id}'>
                            <input type='number' name='quantity' value='{i.Quantity}' min='0' max='{i.Quantity}' style='width:70px'>
                            <button type='submit'>Update</button>
                        </form>
                    </td>
                </tr>");
        }

        sb.Append("</table>");
        return sb.ToString(); 
    }
}
