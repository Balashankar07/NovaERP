using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Inventory.DTOs;
using NovaERP.Domain.Enums;

namespace NovaERP.Application.Interfaces.Services;

public interface IInventoryService
{
    Task<PagedResult<InventoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<InventoryDto?> GetByIdAsync(Guid id);
    Task<List<InventoryDto>> GetByProductIdAsync(Guid productId);
    Task<PagedResult<InventoryDto>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<PagedResult<InventoryTransactionDto>> GetTransactionsAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Called by GoodsReceiptService when a GRN is completed.
    /// Creates or updates inventory records and appends transactions.
    /// </summary>
    Task ProcessGoodsReceiptAsync(Guid grnId, Guid? currentUserId);
}
