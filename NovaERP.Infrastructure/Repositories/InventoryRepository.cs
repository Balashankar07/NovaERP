using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace NovaERP.Infrastructure.Repositories;

public class InventoryRepository : Repository<Inventory>, IInventoryRepository
{
    public InventoryRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByProductAndLocationAsync(Guid productId, Guid warehouseId, Guid? warehouseLocationId)
    {
        return await _dbSet
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.WarehouseLocation)
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId &&
                x.WarehouseLocationId == warehouseLocationId);
    }

    public async Task<List<Inventory>> GetByProductIdAsync(Guid productId)
    {
        return await _dbSet
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.WarehouseLocation)
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.WarehouseId)
            .ToListAsync();
    }

    public async Task<PagedResult<Inventory>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.WarehouseLocation)
            .Where(x => x.WarehouseId == warehouseId)
            .AsQueryable();

        query = ApplySearchAndSort(query, search, sortBy, sortOrder);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Inventory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<Inventory>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .Include(x => x.WarehouseLocation)
            .AsQueryable();

        query = ApplySearchAndSort(query, search, sortBy, sortOrder);

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<Inventory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static IQueryable<Inventory> ApplySearchAndSort(IQueryable<Inventory> query, string? search, string? sortBy, string? sortOrder)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                (x.Product != null && x.Product.Name.Contains(search)) ||
                (x.Product != null && x.Product.ProductCode.Contains(search)) ||
                (x.Warehouse != null && x.Warehouse.WarehouseName.Contains(search)) ||
                (x.WarehouseLocation != null && x.WarehouseLocation.LocationName.Contains(search)));
        }

        bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "quantityonhand" => isDesc ? query.OrderByDescending(x => x.QuantityOnHand) : query.OrderBy(x => x.QuantityOnHand),
                "quantityavailable" => isDesc ? query.OrderByDescending(x => x.QuantityAvailable) : query.OrderBy(x => x.QuantityAvailable),
                "laststockupdate" => isDesc ? query.OrderByDescending(x => x.LastStockUpdate) : query.OrderBy(x => x.LastStockUpdate),
                _ => isDesc ? query.OrderByDescending(x => x.LastStockUpdate) : query.OrderBy(x => x.LastStockUpdate)
            };
        }
        else
        {
            query = query.OrderByDescending(x => x.LastStockUpdate);
        }

        return query;
    }
}
