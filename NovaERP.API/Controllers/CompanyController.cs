using NovaERP.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Features.Companies.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompanyController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    [HasPermission("Companies.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var companies = await _companyService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Companies.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var company = await _companyService.GetByIdAsync(id);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    [HasPermission("Companies.Create")]
    public async Task<IActionResult> Create(CreateCompanyDto dto)
    {
        var company = await _companyService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            company);
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Companies.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateCompanyDto dto)
    {
        var company = await _companyService.UpdateAsync(id, dto);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Companies.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _companyService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}