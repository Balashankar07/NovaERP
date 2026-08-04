using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Units.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitsController(IUnitService unitService)
    {
        _unitService = unitService;
    }

    [HttpGet]
    [HasPermission("Permissions.Units.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var units = await _unitService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", units));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Units.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var unit = await _unitService.GetByIdAsync(id);
        if (unit == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", unit));
    }

    [HttpPost]
    [HasPermission("Permissions.Units.Create")]
    public async Task<IActionResult> Create(CreateUnitDto dto)
    {
        var unit = await _unitService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = unit.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", unit));
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.Units.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateUnitDto dto)
    {
        var unit = await _unitService.UpdateAsync(id, dto);
        if (unit == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", unit));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.Units.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _unitService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
