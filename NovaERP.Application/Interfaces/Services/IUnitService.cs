using NovaERP.Application.Features.Units.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IUnitService
{
    Task<NovaERP.Application.Common.Models.PagedResult<UnitDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<UnitDto?> GetByIdAsync(Guid id);

    Task<UnitDto> CreateAsync(CreateUnitDto dto);

    Task<UnitDto?> UpdateAsync(Guid id, UpdateUnitDto dto);

    Task<bool> DeleteAsync(Guid id);
}
