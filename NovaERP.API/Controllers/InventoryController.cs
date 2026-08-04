using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>GET /api/Inventory — Paginated list with optional search and sort.</summary>
    [HttpGet]
    [HasPermission("Permissions.Inventory.View")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _inventoryService.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(new ApiResponse<object>(true, "Inventory records retrieved successfully.", result));
    }

    /// <summary>GET /api/Inventory/{id} — Single inventory record by ID.</summary>
    [HttpGet("{id:guid}")]
    [HasPermission("Permissions.Inventory.View")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var inventory = await _inventoryService.GetByIdAsync(id);
        if (inventory == null)
            return NotFound(new ApiResponse<object>(false, "Inventory record not found.", null));

        return Ok(new ApiResponse<object>(true, "Inventory record retrieved successfully.", inventory));
    }

    /// <summary>GET /api/Inventory/by-product/{productId} — All inventory records for a product across warehouses.</summary>
    [HttpGet("by-product/{productId:guid}")]
    [HasPermission("Permissions.Inventory.View")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var inventories = await _inventoryService.GetByProductIdAsync(productId);
        return Ok(new ApiResponse<object>(true, "Inventory records for product retrieved successfully.", inventories));
    }

    /// <summary>GET /api/Inventory/by-warehouse/{warehouseId} — Paginated inventory for a specific warehouse.</summary>
    [HttpGet("by-warehouse/{warehouseId:guid}")]
    [HasPermission("Permissions.Inventory.View")]
    public async Task<IActionResult> GetByWarehouse(
        Guid warehouseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null)
    {
        var result = await _inventoryService.GetByWarehouseIdAsync(warehouseId, pageNumber, pageSize, search, sortBy, sortOrder);
        return Ok(new ApiResponse<object>(true, "Inventory records for warehouse retrieved successfully.", result));
    }

    /// <summary>GET /api/Inventory/{id}/transactions — Paginated transaction log for an inventory record.</summary>
    [HttpGet("{id:guid}/transactions")]
    [HasPermission("Permissions.Inventory.Transactions.View")]
    public async Task<IActionResult> GetTransactions(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _inventoryService.GetTransactionsAsync(id, pageNumber, pageSize);
        return Ok(new ApiResponse<object>(true, "Inventory transactions retrieved successfully.", result));
    }
}
