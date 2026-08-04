using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.BOMs.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BOMsController : ControllerBase
{
    private readonly IBOMService _bomService;

    public BOMsController(IBOMService bomService)
    {
        _bomService = bomService;
    }

    [HttpGet]
    [HasPermission("Permissions.BOMs.View")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
    {
        var boms = await _bomService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", boms));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.BOMs.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var bom = await _bomService.GetByIdAsync(id);
        if (bom == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", bom));
    }

    [HttpPost]
    [HasPermission("Permissions.BOMs.Create")]
    public async Task<IActionResult> Create(CreateBOMDto dto)
    {
        try
        {
            var bom = await _bomService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = bom.Id }, ApiResponse.SuccessResponse("Operation completed successfully.", bom));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.BOMs.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateBOMDto dto)
    {
        try
        {
            var bom = await _bomService.UpdateAsync(id, dto);
            if (bom == null) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
            return Ok(ApiResponse.SuccessResponse("Operation completed successfully.", bom));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.BOMs.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _bomService.DeleteAsync(id);
        if (!deleted) return NotFound(ApiResponse.ErrorResponse("Resource not found."));
        return NoContent();
    }
}
