using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class InventoryTransactionRepository : Repository<InventoryTransaction>, IInventoryTransactionRepository
{
    public InventoryTransactionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<InventoryTransaction>> GetByInventoryIdAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10)
    {
        var query = _dbSet
            .Where(x => x.InventoryId == inventoryId)
            .OrderByDescending(x => x.CreatedAt)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<InventoryTransaction>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task AddTransactionAsync(InventoryTransaction transaction)
    {
        await _dbSet.AddAsync(transaction);
    }
}
