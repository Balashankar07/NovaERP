using NovaERP.Application.Common.Models;
using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IWarehouseLocationRepository : IRepository<WarehouseLocation>
{
    Task<bool> ExistsByCodeAsync(Guid warehouseId, string locationCode);
    Task<bool> ExistsByNameAsync(Guid warehouseId, string locationName);
    Task<bool> AnyLocationsInWarehouseAsync(Guid warehouseId);
    Task<List<WarehouseLocation>> GetByWarehouseIdAsync(Guid warehouseId);
    Task<PagedResult<WarehouseLocation>> GetPagedByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
}
