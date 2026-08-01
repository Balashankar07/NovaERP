using NovaERP.Application.Features.Companies.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetAllAsync();

    Task<CompanyDto?> GetByIdAsync(Guid id);

    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);

    Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto);

    Task<bool> DeleteAsync(Guid id);
}