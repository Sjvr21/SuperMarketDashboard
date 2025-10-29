namespace SuperMarketDashboard;

public class InventoryLog
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public int ItemId { get; set; }
    public int OldQuantity { get; set; }
    public int NewQuantity { get; set; }
    public DateTime Timestamp { get; set; }
}
