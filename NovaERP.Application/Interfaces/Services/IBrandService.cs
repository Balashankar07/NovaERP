using NovaERP.Application.Features.Brands.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IBrandService
{
    Task<NovaERP.Application.Common.Models.PagedResult<BrandDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<BrandDto?> GetByIdAsync(Guid id);

    Task<BrandDto> CreateAsync(CreateBrandDto dto);

    Task<BrandDto?> UpdateAsync(Guid id, UpdateBrandDto dto);

    Task<bool> DeleteAsync(Guid id);
}
