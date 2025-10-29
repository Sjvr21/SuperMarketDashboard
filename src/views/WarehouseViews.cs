using System.Collections.Generic;
using System.Text;

namespace SuperMarketDashboard;

public static class WarehouseViews
{
    public static string ItemList(List<WarehouseItem> items, string username)
    {
        var sb = new StringBuilder();
        sb.Append("<h2>Warehouse Inventory</h2>");
        sb.Append("<table border='1' cellpadding='6'>");
        sb.Append("<p><a href='/cart'>🛒 View Cart</a></p>");
        sb.Append("<tr><th>ID</th><th>Name</th><th>Category</th><th>Price</th><th>Stock</th><th>Add to Order</th></tr>");

        foreach (var i in items)
        {
            sb.Append($@"
                <tr>
                    <td>{i.Id}</td>
                    <td>{i.Name}</td>
                    <td>{i.Category}</td>
                    <td>${i.Price:F2}</td>
                    <td>{i.Stock}</td>
                    <td>
                        <form method='POST' action='/warehouse/add'>
                            <input type='hidden' name='id' value='{i.Id}'>
                            <input type='number' name='quantity' value='1' min='1' max='{i.Stock}' style='width:60px'>
                            <button type='submit'>Add</button>
                        </form>
                    </td>
                </tr>");
        }

        sb.Append("</table>");
        return sb.ToString();
    }
}
