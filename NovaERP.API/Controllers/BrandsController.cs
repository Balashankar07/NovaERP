using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Brands.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    [HasPermission("Permissions.Brands.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var brands = await _brandService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", brands));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Brands.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var brand = await _brandService.GetByIdAsync(id);
        if (brand == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", brand));
    }

    [HttpPost]
    [HasPermission("Permissions.Brands.Create")]
    public async Task<IActionResult> Create(CreateBrandDto dto)
    {
        var brand = await _brandService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = brand.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", brand));
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.Brands.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateBrandDto dto)
    {
        var brand = await _brandService.UpdateAsync(id, dto);
        if (brand == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", brand));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.Brands.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _brandService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
