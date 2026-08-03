using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<NovaERP.Application.Common.Models.PagedResult<Company>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<Company?> GetByIdAsync(Guid id);

    Task<Company?> GetByCodeAsync(string code);

    Task AddAsync(Company company);

    Task UpdateAsync(Company company);

    Task DeleteAsync(Company company);
}