using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class InventoryService
{
    private readonly MySqlInventoryRepository repo;
    private readonly MySqlInventoryLogRepository logRepo;

    public InventoryService(MySqlInventoryRepository repo)
    {
        this.repo = repo;
        this.logRepo = new MySqlInventoryLogRepository(repo.ConnectionString);
    }

    public async Task<List<InventoryItem>> ReadAll()
    {
        return await repo.ReadAll();
    }

    public async Task UpdateQuantity(int id, int newQuantity, string username)
    {
        var allItems = await repo.ReadAll();
        var current = allItems.Find(i => i.Id == id);
        if (current == null) return;

        int oldQuantity = current.Quantity;

        // ✅ Only allow decrease
        if (newQuantity < oldQuantity)
        {
            await repo.UpdateQuantity(id, newQuantity);
            await logRepo.Create(username, id, oldQuantity, newQuantity);
            Console.WriteLine($"LOG: {username} reduced item #{id} from {oldQuantity} → {newQuantity}");
        }
        else
        {
            Console.WriteLine($"User '{username}' attempted invalid increase for item #{id} ({oldQuantity} → {newQuantity})");
        }
    }
}
