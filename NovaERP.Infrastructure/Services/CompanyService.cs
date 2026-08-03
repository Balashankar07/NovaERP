using NovaERP.Application.Features.Companies.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public CompanyService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<NovaERP.Application.Common.Models.PagedResult<CompanyDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var companies = await _unitOfWork.Companies.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);

        return new NovaERP.Application.Common.Models.PagedResult<CompanyDto>
        {
            Items = companies.Items.Select(MapToDto).ToList(),
            TotalCount = companies.TotalCount,
            PageNumber = companies.PageNumber,
            PageSize = companies.PageSize
        };
    }

    public async Task<CompanyDto?> GetByIdAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);

        if (company == null)
            return null;

        return MapToDto(company);
    }

    public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
    {
        var existing = await _unitOfWork.Companies.GetByCodeAsync(dto.Code);

        if (existing != null)
            throw new Exception("Company code already exists.");

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            Email = dto.Email,
            Phone = dto.Phone,
            Website = dto.Website,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            LogoUrl = dto.LogoUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Companies.AddAsync(company);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Create", "Company", company.Id.ToString(), newValues: $"Code: {company.Code}, Name: {company.Name}");

        return MapToDto(company);
    }

    public async Task<CompanyDto?> UpdateAsync(Guid id, UpdateCompanyDto dto)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);

        if (company == null)
            return null;

        company.Name = dto.Name;
        company.Email = dto.Email;
        company.Phone = dto.Phone;
        company.Website = dto.Website;
        company.Address = dto.Address;
        company.City = dto.City;
        company.State = dto.State;
        company.Country = dto.Country;
        company.PostalCode = dto.PostalCode;
        company.LogoUrl = dto.LogoUrl;
        company.IsActive = dto.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Companies.UpdateAsync(company);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Update", "Company", company.Id.ToString());

        return MapToDto(company);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id);

        if (company == null)
            return false;

        await _unitOfWork.Companies.DeleteAsync(company);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "Company", company.Id.ToString());

        return true;
    }

    private static CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Code = company.Code,
            Email = company.Email,
            Phone = company.Phone,
            Website = company.Website,
            Address = company.Address,
            City = company.City,
            State = company.State,
            Country = company.Country,
            PostalCode = company.PostalCode,
            LogoUrl = company.LogoUrl,
            IsActive = company.IsActive
        };
    }
}