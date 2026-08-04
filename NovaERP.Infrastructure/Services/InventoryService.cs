using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Inventory.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;
using NovaERP.Domain.Enums;

namespace NovaERP.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public InventoryService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<InventoryDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var result = await _unitOfWork.Inventories.GetAllPagedAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<InventoryDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<InventoryDto?> GetByIdAsync(Guid id)
    {
        var inventory = await _unitOfWork.Inventories.GetByIdAsync(id);
        return inventory == null ? null : MapToDto(inventory);
    }

    public async Task<List<InventoryDto>> GetByProductIdAsync(Guid productId)
    {
        var inventories = await _unitOfWork.Inventories.GetByProductIdAsync(productId);
        return inventories.Select(MapToDto).ToList();
    }

    public async Task<PagedResult<InventoryDto>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var result = await _unitOfWork.Inventories.GetByWarehouseIdAsync(warehouseId, pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<InventoryDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<PagedResult<InventoryTransactionDto>> GetTransactionsAsync(Guid inventoryId, int pageNumber = 1, int pageSize = 10)
    {
        var result = await _unitOfWork.InventoryTransactions.GetByInventoryIdAsync(inventoryId, pageNumber, pageSize);
        return new PagedResult<InventoryTransactionDto>
        {
            Items = result.Items.Select(MapTransactionToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task ProcessGoodsReceiptAsync(Guid grnId, Guid? currentUserId)
    {
        var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(grnId);
        if (grn == null)
            throw new Exception($"GRN with ID {grnId} not found.");

        // Receive goods into the default warehouse (GRN does not carry a target warehouse/location)
        var warehouseId = await GetDefaultWarehouseIdAsync();

        foreach (var item in grn.Items)
        {
            decimal receivedQty = item.ReceivedQuantity - item.RejectedQuantity;
            if (receivedQty <= 0) continue;

            var inventory = await _unitOfWork.Inventories.GetByProductAndLocationAsync(item.ProductId, warehouseId, null);

            if (inventory == null)
            {
                // Create new inventory record
                inventory = new Inventory
                {
                    ProductId = item.ProductId,
                    WarehouseId = warehouseId,
                    WarehouseLocationId = null,
                    QuantityOnHand = receivedQty,
                    QuantityReserved = 0,
                    QuantityAvailable = receivedQty,
                    LastStockUpdate = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.Inventories.AddAsync(inventory);
                await _unitOfWork.SaveChangesAsync();

                await _auditLogger.LogAsync("Create", "Inventory", inventory.Id.ToString(),
                    newValues: $"ProductId: {item.ProductId}, WarehouseId: {warehouseId}, QuantityOnHand: {receivedQty}");
            }
            else
            {
                var oldQty = inventory.QuantityOnHand;
                inventory.QuantityOnHand += receivedQty;
                inventory.QuantityAvailable = inventory.QuantityOnHand - inventory.QuantityReserved;
                inventory.LastStockUpdate = DateTime.UtcNow;

                _unitOfWork.Inventories.Update(inventory);
                await _unitOfWork.SaveChangesAsync();

                await _auditLogger.LogAsync("Update", "Inventory", inventory.Id.ToString(),
                    oldValues: $"QuantityOnHand: {oldQty}",
                    newValues: $"QuantityOnHand: {inventory.QuantityOnHand}");
            }

            // Append-only inventory transaction
            var transaction = new InventoryTransaction
            {
                InventoryId = inventory.Id,
                TransactionType = InventoryTransactionType.GoodsReceipt,
                ReferenceType = InventoryReferenceType.GoodsReceipt,
                ReferenceId = grnId,
                Quantity = receivedQty,
                BalanceAfter = inventory.QuantityOnHand,
                Remarks = $"Received from GRN: {grn.GRNNumber}",
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.InventoryTransactions.AddTransactionAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task<Guid> GetDefaultWarehouseIdAsync()
    {
        var defaultWarehouse = await _unitOfWork.Warehouses.GetDefaultWarehouseAsync();
        if (defaultWarehouse == null)
            throw new Exception("No default warehouse is configured. Please set a default warehouse before processing goods receipts.");
        return defaultWarehouse.Id;
    }

    private static InventoryDto MapToDto(Inventory inv) => new()
    {
        Id = inv.Id,
        ProductId = inv.ProductId,
        ProductCode = inv.Product?.ProductCode ?? string.Empty,
        ProductName = inv.Product?.Name ?? string.Empty,
        WarehouseId = inv.WarehouseId,
        WarehouseName = inv.Warehouse?.WarehouseName ?? string.Empty,
        WarehouseLocationId = inv.WarehouseLocationId,
        LocationName = inv.WarehouseLocation?.LocationName,
        QuantityOnHand = inv.QuantityOnHand,
        QuantityReserved = inv.QuantityReserved,
        QuantityAvailable = inv.QuantityAvailable,
        ReorderLevel = inv.ReorderLevel,
        MinimumLevel = inv.MinimumLevel,
        MaximumLevel = inv.MaximumLevel,
        LastStockUpdate = inv.LastStockUpdate,
        IsActive = inv.IsActive,
        CreatedAt = inv.CreatedAt,
        UpdatedAt = inv.UpdatedAt
    };

    private static InventoryTransactionDto MapTransactionToDto(InventoryTransaction t) => new()
    {
        Id = t.Id,
        InventoryId = t.InventoryId,
        TransactionType = t.TransactionType.ToString(),
        ReferenceType = t.ReferenceType.ToString(),
        ReferenceId = t.ReferenceId,
        Quantity = t.Quantity,
        BalanceAfter = t.BalanceAfter,
        Remarks = t.Remarks,
        CreatedBy = t.CreatedBy,
        CreatedAt = t.CreatedAt
    };
}
