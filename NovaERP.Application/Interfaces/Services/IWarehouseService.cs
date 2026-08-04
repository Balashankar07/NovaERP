using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Warehouses.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IWarehouseService
{
    Task<PagedResult<WarehouseDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<WarehouseDto?> GetByIdAsync(Guid id);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
    Task<WarehouseDto?> UpdateAsync(Guid id, UpdateWarehouseDto dto);
    Task<bool> DeleteAsync(Guid id);
}
