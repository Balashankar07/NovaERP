using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductionOrders.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductionOrdersController : ControllerBase
{
    private readonly IProductionOrderService _productionOrderService;
    private readonly ICurrentUserService _currentUserService;

    public ProductionOrdersController(IProductionOrderService productionOrderService, ICurrentUserService currentUserService)
    {
        _productionOrderService = productionOrderService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [HasPermission("Permissions.ProductionOrders.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _productionOrderService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(new ApiResponse<object>(true, "Production Orders retrieved successfully.", result));
    }

    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.ProductionOrders.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _productionOrderService.GetByIdAsync(id);
        if (order == null)
            return NotFound(new ApiResponse<object>(false, "Production Order not found.", null));

        return Ok(new ApiResponse<object>(true, "Production Order retrieved successfully.", order));
    }

    [HttpPost]
    [HasPermission("Permissions.ProductionOrders.Create")]
    public async Task<IActionResult> Create([FromBody] CreateProductionOrderDto dto)
    {
        var result = await _productionOrderService.CreateAsync(dto, _currentUserService.UserId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new ApiResponse<object>(true, "Production Order created successfully.", result));
    }

    [HttpPut("{id:guid}")]
    [HasPermission("Permissions.ProductionOrders.Update")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductionOrderDto dto)
    {
        var result = await _productionOrderService.UpdateAsync(id, dto, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order updated successfully.", result));
    }

    [HttpDelete("{id:guid}")]
    [HasPermission("Permissions.ProductionOrders.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productionOrderService.DeleteAsync(id, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order deleted successfully.", null));
    }

    [HttpPost("{id:guid}/release")]
    [HasPermission("Permissions.ProductionOrders.Release")]
    public async Task<IActionResult> Release(Guid id)
    {
        var result = await _productionOrderService.ReleaseAsync(id, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order released successfully.", result));
    }

    public class StartProductionOrderRequest
    {
        public decimal StartedQuantity { get; set; }
    }

    [HttpPost("{id:guid}/start")]
    [HasPermission("Permissions.ProductionOrders.Start")]
    public async Task<IActionResult> Start(Guid id, [FromBody] StartProductionOrderRequest request)
    {
        var result = await _productionOrderService.StartAsync(id, request.StartedQuantity, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order started successfully.", result));
    }

    public class CompleteProductionOrderRequest
    {
        public decimal CompletedQuantity { get; set; }
        public decimal RejectedQuantity { get; set; }
    }

    [HttpPost("{id:guid}/complete")]
    [HasPermission("Permissions.ProductionOrders.Complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompleteProductionOrderRequest request)
    {
        var result = await _productionOrderService.CompleteAsync(id, request.CompletedQuantity, request.RejectedQuantity, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order completed successfully.", result));
    }

    public class CancelProductionOrderRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    [HttpPost("{id:guid}/cancel")]
    [HasPermission("Permissions.ProductionOrders.Cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] CancelProductionOrderRequest request)
    {
        var result = await _productionOrderService.CancelAsync(id, request.Reason, _currentUserService.UserId);
        return Ok(new ApiResponse<object>(true, "Production Order cancelled successfully.", result));
    }
}
