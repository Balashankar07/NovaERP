using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.API.Extensions;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Warranties.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarrantyClaimsController : ControllerBase
{
    private readonly IWarrantyService _warrantyService;

    public WarrantyClaimsController(IWarrantyService warrantyService)
    {
        _warrantyService = warrantyService;
    }

    [HttpGet]
    [HasPermission("Permissions.Warranties.View")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<WarrantyClaimDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _warrantyService.GetAllClaimsAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Warranty claims retrieved successfully.", result));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Warranties.View")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _warrantyService.GetClaimByIdAsync(id);
        return Ok(ApiResponse.SuccessResponse("Warranty claim retrieved successfully.", result));
    }

    [HttpPost]
    [HasPermission("Permissions.Warranties.Claim")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateWarrantyClaimDto request)
    {
        var result = await _warrantyService.CreateClaimAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse.SuccessResponse("Warranty claim created successfully.", result));
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.Warranties.Update")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarrantyClaimDto request)
    {
        var result = await _warrantyService.UpdateClaimAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Warranty claim updated successfully.", result));
    }

    [HttpPut("{id:guid}/approve")]
    [HasPermission("Permissions.Warranties.Approve")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var request = new UpdateWarrantyClaimDto { Status = Domain.Enums.WarrantyClaimStatus.Approved };
        var result = await _warrantyService.UpdateClaimAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Warranty claim approved successfully.", result));
    }

    [HttpPut("{id:guid}/reject")]
    [HasPermission("Permissions.Warranties.Reject")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var request = new UpdateWarrantyClaimDto { Status = Domain.Enums.WarrantyClaimStatus.Rejected };
        var result = await _warrantyService.UpdateClaimAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Warranty claim rejected successfully.", result));
    }

    [HttpPut("{id:guid}/resolve")]
    [HasPermission("Permissions.Warranties.Resolve")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] UpdateWarrantyClaimDto request)
    {
        request.Status = Domain.Enums.WarrantyClaimStatus.Resolved;
        var result = await _warrantyService.UpdateClaimAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Warranty claim resolved successfully.", result));
    }

    [HttpPut("{id:guid}/close")]
    [HasPermission("Permissions.Warranties.Close")]
    [ProducesResponseType(typeof(ApiResponse<WarrantyClaimDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Close(Guid id)
    {
        var request = new UpdateWarrantyClaimDto { Status = Domain.Enums.WarrantyClaimStatus.Closed };
        var result = await _warrantyService.UpdateClaimAsync(id, request);
        return Ok(ApiResponse.SuccessResponse("Warranty claim closed successfully.", result));
    }
}
