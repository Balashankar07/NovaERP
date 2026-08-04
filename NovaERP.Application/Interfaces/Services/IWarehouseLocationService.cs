using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.WarehouseLocations.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IWarehouseLocationService
{
    Task<PagedResult<WarehouseLocationDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<PagedResult<WarehouseLocationDto>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<WarehouseLocationDto?> GetByIdAsync(Guid id);
    Task<WarehouseLocationDto> CreateAsync(CreateWarehouseLocationDto dto);
    Task<WarehouseLocationDto?> UpdateAsync(Guid id, UpdateWarehouseLocationDto dto);
    Task<bool> DeleteAsync(Guid id);
}
