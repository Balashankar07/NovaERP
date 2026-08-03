using NovaERP.Application.Features.Companies.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface ICompanyService
{
    Task<NovaERP.Application.Common.Models.PagedResult<CompanyDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

    Task<CompanyDto?> GetByIdAsync(Guid id);

    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);

    Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto);

    Task<bool> DeleteAsync(Guid id);
}