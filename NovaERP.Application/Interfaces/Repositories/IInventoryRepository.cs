using NovaERP.Application.Common.Models;
using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<Inventory?> GetByProductAndLocationAsync(Guid productId, Guid warehouseId, Guid? warehouseLocationId);
    Task<List<Inventory>> GetByProductIdAsync(Guid productId);
    Task<PagedResult<Inventory>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<PagedResult<Inventory>> GetAllPagedAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
}
