using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class InventoryLogService
{
    private readonly MySqlInventoryLogRepository repo;

    public InventoryLogService(MySqlInventoryLogRepository repo)
    {
        this.repo = repo;
    }

    public async Task<List<InventoryLog>> ReadAll()
    {
        return await repo.ReadAll();
    }

    public async Task CreateLog(string username, int itemId, int oldQty, int newQty)
    {
        await repo.CreateLog(username, itemId, oldQty, newQty);
    }
}
