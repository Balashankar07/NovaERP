using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Controllers;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.QualityInspections.DTOs;
using NovaERP.Application.Interfaces.Services;

using System.Security.Claims;

namespace NovaERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QualityInspectionsController : ControllerBase
{
    private readonly IQualityInspectionService _qualityInspectionService;

    public QualityInspectionsController(IQualityInspectionService qualityInspectionService)
    {
        _qualityInspectionService = qualityInspectionService;
    }

    [HttpGet]
    [HasPermission("Permissions.QualityInspection.View")]
    public async Task<ActionResult<ApiResponse<PagedResult<QualityInspectionDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _qualityInspectionService.GetQualityInspectionsAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Success", result));
    }

    [HttpGet("{id}")]
    [HasPermission("Permissions.QualityInspection.View")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> GetById(Guid id)
    {
        var result = await _qualityInspectionService.GetQualityInspectionByIdAsync(id);
        return Ok(ApiResponse.SuccessResponse("Success", result));
    }

    [HttpPost]
    [HasPermission("Permissions.QualityInspection.Create")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> Create([FromBody] CreateQualityInspectionDto dto)
    {
        var result = await _qualityInspectionService.CreateAsync(dto, GetCurrentUserId());
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse.SuccessResponse("Quality Inspection created successfully.", result));
    }

    [HttpPut("{id}")]
    [HasPermission("Permissions.QualityInspection.Update")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> Update(Guid id, [FromBody] UpdateQualityInspectionDto dto)
    {
        var result = await _qualityInspectionService.UpdateAsync(id, dto, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Quality Inspection updated successfully.", result));
    }

    [HttpDelete("{id}")]
    [HasPermission("Permissions.QualityInspection.Delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        await _qualityInspectionService.DeleteAsync(id, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Quality Inspection deleted successfully.", true));
    }

    [HttpPost("{id}/start")]
    [HasPermission("Permissions.QualityInspection.Start")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> Start(Guid id)
    {
        var result = await _qualityInspectionService.StartAsync(id, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Quality Inspection started successfully.", result));
    }

    [HttpPost("{id}/complete")]
    [HasPermission("Permissions.QualityInspection.Complete")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> Complete(Guid id)
    {
        var result = await _qualityInspectionService.CompleteAsync(id, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Quality Inspection completed successfully.", result));
    }

    [HttpPost("{id}/cancel")]
    [HasPermission("Permissions.QualityInspection.Cancel")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> Cancel(Guid id, [FromQuery] string reason)
    {
        var result = await _qualityInspectionService.CancelAsync(id, reason, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Quality Inspection cancelled successfully.", result));
    }

    [HttpPost("{id}/defects")]
    [HasPermission("Permissions.QualityInspection.Update")]
    public async Task<ActionResult<ApiResponse<QualityInspectionDto>>> AddDefect(Guid id, [FromBody] CreateQualityDefectDto dto)
    {
        var result = await _qualityInspectionService.AddDefectAsync(id, dto, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Defect added successfully.", result));
    }

    [HttpDelete("{id}/defects/{defectId}")]
    [HasPermission("Permissions.QualityInspection.Update")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveDefect(Guid id, Guid defectId)
    {
        await _qualityInspectionService.RemoveDefectAsync(id, defectId, GetCurrentUserId());
        return Ok(ApiResponse.SuccessResponse("Defect removed successfully.", true));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdString, out Guid userId))
        {
            return userId;
        }
        return null;
    }
}
