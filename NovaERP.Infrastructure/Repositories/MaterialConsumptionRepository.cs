using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class MaterialConsumptionRepository : Repository<MaterialConsumption>, IMaterialConsumptionRepository
{
    public MaterialConsumptionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<MaterialConsumption>> GetByProductionExecutionIdAsync(Guid executionId)
    {
        return await _dbSet
            .Include(x => x.Product)
            .Include(x => x.Inventory)
            .Where(x => x.ProductionExecutionId == executionId)
            .ToListAsync();
    }

    public async Task AddRangeAsync(IEnumerable<MaterialConsumption> consumptions)
    {
        await _dbSet.AddRangeAsync(consumptions);
    }
}
