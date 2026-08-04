using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.WarehouseLocations.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WarehouseLocationsController : ControllerBase
{
    private readonly IWarehouseLocationService _locationService;

    public WarehouseLocationsController(IWarehouseLocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    [HasPermission("Permissions.WarehouseLocations.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var result = await _locationService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(new ApiResponse<object>(true, "Warehouse Locations retrieved successfully.", result));
    }

    [HttpGet("warehouse/{warehouseId}")]
    [HasPermission("Permissions.WarehouseLocations.View")]
    public async Task<IActionResult> GetByWarehouseId(Guid warehouseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var result = await _locationService.GetByWarehouseIdAsync(warehouseId, pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(new ApiResponse<object>(true, "Warehouse Locations retrieved successfully.", result));
    }

    [HttpGet("{id}")]
    [HasPermission("Permissions.WarehouseLocations.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var location = await _locationService.GetByIdAsync(id);
        if (location == null)
            return NotFound(new ApiResponse<object>(false, "Warehouse Location not found.", null));

        return Ok(new ApiResponse<object>(true, "Warehouse Location retrieved successfully.", location));
    }

    [HttpPost]
    [HasPermission("Permissions.WarehouseLocations.Create")]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseLocationDto dto)
    {
        var location = await _locationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, new ApiResponse<object>(true, "Warehouse Location created successfully.", location));
    }

    [HttpPut("{id}")]
    [HasPermission("Permissions.WarehouseLocations.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseLocationDto dto)
    {
        var location = await _locationService.UpdateAsync(id, dto);
        if (location == null)
            return NotFound(new ApiResponse<object>(false, "Warehouse Location not found.", null));

        return Ok(new ApiResponse<object>(true, "Warehouse Location updated successfully.", location));
    }

    [HttpDelete("{id}")]
    [HasPermission("Permissions.WarehouseLocations.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _locationService.DeleteAsync(id);
        if (!result)
            return NotFound(new ApiResponse<object>(false, "Warehouse Location not found.", null));

        return Ok(new ApiResponse<object>(true, "Warehouse Location deleted successfully.", null));
    }
}
