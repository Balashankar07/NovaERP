using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;
using System.Linq.Expressions;

namespace NovaERP.Infrastructure.Repositories;

public class WarehouseLocationRepository : Repository<WarehouseLocation>, IWarehouseLocationRepository
{
    public WarehouseLocationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByCodeAsync(Guid warehouseId, string locationCode)
    {
        return await _dbSet.AnyAsync(x => x.WarehouseId == warehouseId && x.LocationCode == locationCode);
    }

    public async Task<bool> ExistsByNameAsync(Guid warehouseId, string locationName)
    {
        return await _dbSet.AnyAsync(x => x.WarehouseId == warehouseId && x.LocationName == locationName);
    }

    public async Task<bool> AnyLocationsInWarehouseAsync(Guid warehouseId)
    {
        return await _dbSet.AnyAsync(x => x.WarehouseId == warehouseId);
    }

    public async Task<List<WarehouseLocation>> GetByWarehouseIdAsync(Guid warehouseId)
    {
        return await _dbSet
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<PagedResult<WarehouseLocation>> GetPagedByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == warehouseId)
            .AsQueryable();

        return await ApplyFiltersAndPaginationAsync(query, pageNumber, pageSize, search, sortBy, sortOrder);
    }

    public override async Task<PagedResult<WarehouseLocation>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _dbSet
            .Include(x => x.Warehouse)
            .AsQueryable();

        return await ApplyFiltersAndPaginationAsync(query, pageNumber, pageSize, search, sortBy, sortOrder);
    }
    
    private async Task<PagedResult<WarehouseLocation>> ApplyFiltersAndPaginationAsync(IQueryable<WarehouseLocation> query, int pageNumber, int pageSize, string? search, string? sortBy, string? sortOrder)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.LocationCode.Contains(search) || 
                                     x.LocationName.Contains(search) || 
                                     (x.Zone != null && x.Zone.Contains(search)) ||
                                     (x.Warehouse != null && x.Warehouse.WarehouseName.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            
            Expression<Func<WarehouseLocation, object>> keySelector = sortBy.ToLower() switch
            {
                "code" => x => x.LocationCode,
                "name" => x => x.LocationName,
                "warehouse" => x => x.Warehouse!.WarehouseName,
                "zone" => x => x.Zone ?? string.Empty,
                "createdat" => x => x.CreatedAt,
                "isactive" => x => x.IsActive,
                _ => x => x.CreatedAt
            };

            query = isDesc ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
        }
        else
        {
            query = query.OrderBy(x => x.LocationCode);
        }

        var totalCount = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<WarehouseLocation>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
