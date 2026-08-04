using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IUnitRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<Unit>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<Unit?> GetByIdAsync(Guid id);

    Task AddAsync(Unit unit);

    Task UpdateAsync(Unit unit);

    Task DeleteAsync(Unit unit);
}
