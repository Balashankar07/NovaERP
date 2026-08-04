using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Suppliers.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    [HttpGet]
    [HasPermission("Permissions.Suppliers.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var suppliers = await _supplierService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", suppliers));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Suppliers.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", supplier));
    }

    [HttpPost]
    [HasPermission("Permissions.Suppliers.Create")]
    public async Task<IActionResult> Create(CreateSupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", supplier));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.Suppliers.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.UpdateAsync(id, dto);
            if (supplier == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
            return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", supplier));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.Suppliers.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _supplierService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
