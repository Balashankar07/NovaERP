using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<Brand>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<Brand?> GetByIdAsync(Guid id);

    Task AddAsync(Brand brand);

    Task UpdateAsync(Brand brand);

    Task DeleteAsync(Brand brand);
}
