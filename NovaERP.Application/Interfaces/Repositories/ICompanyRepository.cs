using NovaERP.Domain.Entities;

namespace NovaERP.Application.Interfaces.Repositories;

public interface ICompanyRepository
{
    Task<List<Company>> GetAllAsync();

    Task<Company?> GetByIdAsync(Guid id);

    Task<Company?> GetByCodeAsync(string code);

    Task AddAsync(Company company);

    Task UpdateAsync(Company company);

    Task DeleteAsync(Company company);
}