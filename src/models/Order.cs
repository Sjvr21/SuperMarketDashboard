namespace SuperMarketDashboard;

public class Order
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public decimal TotalPrice { get; set; }
    public decimal Shipping { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Pending";

}
