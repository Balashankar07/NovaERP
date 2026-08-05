using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductionExecutions.DTOs;
using NovaERP.Application.Interfaces.Services;
using NovaERP.API.Authorization;
using System.Security.Claims;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductionExecutionsController : ControllerBase
{
    private readonly IProductionExecutionService _productionExecutionService;

    public ProductionExecutionsController(IProductionExecutionService productionExecutionService)
    {
        _productionExecutionService = productionExecutionService;
    }

    [HttpGet]
    [HasPermission("Permissions.ProductionExecution.View")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductionExecutionDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _productionExecutionService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(ApiResponse.SuccessResponse("Success", result));
    }

    [HttpGet("{id}")]
    [HasPermission("Permissions.ProductionExecution.View")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> GetById(Guid id)
    {
        var execution = await _productionExecutionService.GetByIdAsync(id);
        if (execution == null)
            return NotFound(ApiResponse.ErrorResponse<ProductionExecutionDto>("Production Execution not found", null!));

        return Ok(ApiResponse.SuccessResponse("Success", execution));
    }

    [HttpPost]
    [HasPermission("Permissions.ProductionExecution.Create")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> Create([FromBody] CreateProductionExecutionDto request)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.CreateAsync(request, currentUserId);
        
        return CreatedAtAction(nameof(GetById), new { id = execution.Id }, ApiResponse.SuccessResponse("Production Execution created successfully.", execution));
    }

    [HttpPut("{id}")]
    [HasPermission("Permissions.ProductionExecution.Update")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> Update(Guid id, [FromBody] UpdateProductionExecutionDto request)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.UpdateAsync(id, request, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Production Execution updated successfully.", execution));
    }

    [HttpDelete("{id}")]
    [HasPermission("Permissions.ProductionExecution.Delete")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var result = await _productionExecutionService.DeleteAsync(id, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Production Execution deleted successfully.", result));
    }

    [HttpPost("{id}/start")]
    [HasPermission("Permissions.ProductionExecution.Start")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> Start(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.StartAsync(id, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Production Execution started successfully.", execution));
    }

    [HttpPost("{id}/consume")]
    [HasPermission("Permissions.ProductionExecution.Consume")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> ConsumeMaterials(Guid id)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.ConsumeMaterialsAsync(id, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Materials consumed successfully.", execution));
    }

    [HttpPost("{id}/complete")]
    [HasPermission("Permissions.ProductionExecution.Complete")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> Complete(Guid id, [FromBody] CompleteProductionExecutionDto request)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.CompleteAsync(id, request, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Production Execution completed successfully.", execution));
    }

    [HttpPost("{id}/cancel")]
    [HasPermission("Permissions.ProductionExecution.Cancel")]
    public async Task<ActionResult<ApiResponse<ProductionExecutionDto>>> Cancel(Guid id, [FromBody] CancelProductionExecutionRequest request)
    {
        var currentUserId = GetCurrentUserId();
        var execution = await _productionExecutionService.CancelAsync(id, request.Reason, currentUserId);
        
        return Ok(ApiResponse.SuccessResponse("Production Execution cancelled successfully.", execution));
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

public class CancelProductionExecutionRequest
{
    public string Reason { get; set; } = string.Empty;
}
