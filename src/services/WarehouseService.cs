using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperMarketDashboard;

public class WarehouseService
{
    private readonly MySqlWarehouseRepository repo;

    public WarehouseService(MySqlWarehouseRepository repo)
    {
        this.repo = repo;
    }

    public async Task<List<WarehouseItem>> ReadAll()
    {
        return await repo.ReadAll();
    }
}
