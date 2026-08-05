using NovaERP.Application.Common.Exceptions;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductionExecutions.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;
using NovaERP.Domain.Enums;

namespace NovaERP.Infrastructure.Services;

public class ProductionExecutionService : IProductionExecutionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ProductionExecutionService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<ProductionExecutionDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var items = await _unitOfWork.ProductionExecutions.GetAllPagedAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        var dtos = items.Items.Select(MapToDto).ToList();

        return new PagedResult<ProductionExecutionDto>
        {
            Items = dtos,
            TotalCount = items.TotalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductionExecutionDto?> GetByIdAsync(Guid id)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        return execution == null ? null : MapToDto(execution);
    }

    public async Task<ProductionExecutionDto> CreateAsync(CreateProductionExecutionDto dto, Guid? currentUserId)
    {
        var order = await _unitOfWork.ProductionOrders.GetByIdAsync(dto.ProductionOrderId);
        if (order == null)
            throw new KeyNotFoundException($"ProductionOrder {dto.ProductionOrderId} not found");

        if (order.Status != ProductionOrderStatus.Released && order.Status != ProductionOrderStatus.InProgress)
            throw new BadRequestException("Executions can only be created for Released or InProgress Production Orders.");

        var execution = new ProductionExecution
        {
            ProductionOrderId = dto.ProductionOrderId,
            Remarks = dto.Remarks,
            Status = ProductionExecutionStatus.Draft,
            ExecutionNumber = $"PE-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ProductionExecutions.AddAsync(execution);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("Create", "ProductionExecution", execution.Id.ToString(), newValues: $"ExecutionNumber: {execution.ExecutionNumber}, OrderId: {execution.ProductionOrderId}");

        return MapToDto(execution);
    }

    public async Task<ProductionExecutionDto> UpdateAsync(Guid id, UpdateProductionExecutionDto dto, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status == ProductionExecutionStatus.Completed || execution.Status == ProductionExecutionStatus.Cancelled)
            throw new BadRequestException($"Cannot update a {execution.Status} execution.");

        execution.Remarks = dto.Remarks;
        execution.UpdatedBy = currentUserId;
        execution.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ProductionExecutions.Update(execution);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("Update", "ProductionExecution", execution.Id.ToString(), newValues: $"Remarks updated");

        return MapToDto(execution);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status != ProductionExecutionStatus.Draft)
            throw new BadRequestException("Only Draft executions can be deleted.");

        _unitOfWork.ProductionExecutions.Delete(execution);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("Delete", "ProductionExecution", execution.Id.ToString(), oldValues: $"ExecutionNumber: {execution.ExecutionNumber}");

        return true;
    }

    public async Task<ProductionExecutionDto> StartAsync(Guid id, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status != ProductionExecutionStatus.Draft)
            throw new BadRequestException("Only Draft executions can be started.");

        execution.Status = ProductionExecutionStatus.Started;
        execution.StartedAt = DateTime.UtcNow;
        execution.UpdatedBy = currentUserId;
        execution.UpdatedAt = DateTime.UtcNow;
        
        var order = await _unitOfWork.ProductionOrders.GetByIdAsync(execution.ProductionOrderId);
        if (order != null && order.Status == ProductionOrderStatus.Released)
        {
            order.Status = ProductionOrderStatus.InProgress;
            order.ActualStartDate = DateTime.UtcNow;
            _unitOfWork.ProductionOrders.Update(order);
        }

        _unitOfWork.ProductionExecutions.Update(execution);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "ProductionExecution", execution.Id.ToString(), oldValues: "Draft", newValues: "Started");

        return MapToDto(execution);
    }

    public async Task<ProductionExecutionDto> ConsumeMaterialsAsync(Guid id, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status != ProductionExecutionStatus.Started)
            throw new BadRequestException("Only Started executions can consume materials.");

        if (execution.MaterialConsumptions.Any())
            throw new BadRequestException("Materials have already been consumed for this execution.");

        var order = await _unitOfWork.ProductionOrders.GetByIdAsync(execution.ProductionOrderId);
        if (order == null) throw new KeyNotFoundException("Associated Production Order not found");

        var bom = await _unitOfWork.BOMs.GetActiveByProductIdAsync(order.ProductId);
        if (bom == null)
            throw new BadRequestException($"No active BOM found for Product {order.ProductId}.");

        var executionQuantity = order.PlannedQuantity; // Basis for consumption

        foreach (var item in bom.BOMItems)
        {
            var requiredQty = item.Quantity * executionQuantity;
            var inventories = await _unitOfWork.Inventories.GetByProductIdAsync(item.RawMaterialProductId);
            
            var totalAvailable = inventories.Sum(x => x.QuantityAvailable);
            if (totalAvailable < requiredQty)
                throw new BadRequestException($"Insufficient inventory for Product {item.RawMaterialProduct?.Name}. Required: {requiredQty}, Available: {totalAvailable}");

            decimal remainingToConsume = requiredQty;
            foreach (var inv in inventories)
            {
                if (remainingToConsume <= 0) break;
                if (inv.QuantityAvailable <= 0) continue;

                var consumeQty = Math.Min(inv.QuantityAvailable, remainingToConsume);
                
                var consumption = new MaterialConsumption
                {
                    ProductionExecutionId = execution.Id,
                    ProductId = item.RawMaterialProductId,
                    InventoryId = inv.Id,
                    RequiredQuantity = requiredQty,
                    ConsumedQuantity = consumeQty,
                    VarianceQuantity = 0,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.MaterialConsumptions.AddAsync(consumption);

                inv.QuantityAvailable -= consumeQty;
                inv.QuantityOnHand -= consumeQty;
                inv.LastStockUpdate = DateTime.UtcNow;
                _unitOfWork.Inventories.Update(inv);

                var transaction = new InventoryTransaction
                {
                    InventoryId = inv.Id,
                    TransactionType = InventoryTransactionType.ProductionIssue,
                    ReferenceType = InventoryReferenceType.Production,
                    ReferenceId = execution.Id,
                    Quantity = -consumeQty, // Negative for issue
                    BalanceAfter = inv.QuantityOnHand,
                    Remarks = $"Consumed for Execution {execution.ExecutionNumber}",
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.InventoryTransactions.AddAsync(transaction);

                remainingToConsume -= consumeQty;
            }
        }

        execution.UpdatedBy = currentUserId;
        execution.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ProductionExecutions.Update(execution);
        
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("Consume", "ProductionExecution", execution.Id.ToString(), newValues: "Materials consumed");

        return MapToDto(execution);
    }

    public async Task<ProductionExecutionDto> CompleteAsync(Guid id, CompleteProductionExecutionDto dto, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status != ProductionExecutionStatus.Started)
            throw new BadRequestException("Only Started executions can be completed.");

        if (!execution.MaterialConsumptions.Any())
            throw new BadRequestException("Cannot complete execution without consuming materials first.");

        var order = await _unitOfWork.ProductionOrders.GetByIdAsync(execution.ProductionOrderId);
        if (order == null) throw new KeyNotFoundException("Associated Production Order not found");

        execution.ProducedQuantity = dto.ProducedQuantity;
        execution.RejectedQuantity = dto.RejectedQuantity;
        execution.Status = ProductionExecutionStatus.Completed;
        execution.CompletedAt = DateTime.UtcNow;
        execution.UpdatedBy = currentUserId;
        execution.UpdatedAt = DateTime.UtcNow;

        // Increase finished goods inventory
        // We will just add to the default warehouse or the first available warehouse.
        var warehouse = await _unitOfWork.Warehouses.GetDefaultWarehouseAsync();
        if (warehouse == null)
            throw new BadRequestException("No warehouse found to store finished goods.");

        var fgInventory = await _unitOfWork.Inventories.GetByProductAndLocationAsync(order.ProductId, warehouse.Id, null);
        if (fgInventory == null)
        {
            fgInventory = new Inventory
            {
                ProductId = order.ProductId,
                WarehouseId = warehouse.Id,
                QuantityOnHand = dto.ProducedQuantity,
                QuantityAvailable = dto.ProducedQuantity,
                LastStockUpdate = DateTime.UtcNow,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Inventories.AddAsync(fgInventory);
        }
        else
        {
            fgInventory.QuantityOnHand += dto.ProducedQuantity;
            fgInventory.QuantityAvailable += dto.ProducedQuantity;
            fgInventory.LastStockUpdate = DateTime.UtcNow;
            _unitOfWork.Inventories.Update(fgInventory);
        }

        // We must ensure the inventory is saved so it has an ID before adding transaction if it was newly created
        if (fgInventory.Id == Guid.Empty)
        {
             // Wait, if it's new, EF Core will generate the ID on SaveChanges.
             // But we need the ID for the transaction. Let's just generate a Guid.
             fgInventory.Id = Guid.NewGuid();
        }

        var transaction = new InventoryTransaction
        {
            InventoryId = fgInventory.Id,
            TransactionType = InventoryTransactionType.ProductionReceipt,
            ReferenceType = InventoryReferenceType.Production,
            ReferenceId = execution.Id,
            Quantity = dto.ProducedQuantity,
            BalanceAfter = fgInventory.QuantityOnHand,
            Remarks = $"Produced from Execution {execution.ExecutionNumber}",
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.InventoryTransactions.AddAsync(transaction);

        order.CompletedQuantity += dto.ProducedQuantity;
        order.RejectedQuantity += dto.RejectedQuantity;
        order.Status = ProductionOrderStatus.Completed;
        order.ActualEndDate = DateTime.UtcNow;
        _unitOfWork.ProductionOrders.Update(order);

        _unitOfWork.ProductionExecutions.Update(execution);
        
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "ProductionExecution", execution.Id.ToString(), oldValues: "Started", newValues: "Completed");

        return MapToDto(execution);
    }

    public async Task<ProductionExecutionDto> CancelAsync(Guid id, string reason, Guid? currentUserId)
    {
        var execution = await _unitOfWork.ProductionExecutions.GetByIdAsync(id);
        if (execution == null)
            throw new KeyNotFoundException($"ProductionExecution {id} not found");

        if (execution.Status == ProductionExecutionStatus.Completed || execution.Status == ProductionExecutionStatus.Cancelled)
            throw new BadRequestException($"Cannot cancel a {execution.Status} execution.");

        execution.Status = ProductionExecutionStatus.Cancelled;
        execution.Remarks = string.IsNullOrWhiteSpace(execution.Remarks) ? reason : $"{execution.Remarks} | Cancelled: {reason}";
        execution.UpdatedBy = currentUserId;
        execution.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ProductionExecutions.Update(execution);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "ProductionExecution", execution.Id.ToString(), oldValues: execution.Status.ToString(), newValues: "Cancelled");

        return MapToDto(execution);
    }

    private static ProductionExecutionDto MapToDto(ProductionExecution e) => new()
    {
        Id = e.Id,
        ExecutionNumber = e.ExecutionNumber,
        ProductionOrderId = e.ProductionOrderId,
        StartedAt = e.StartedAt,
        CompletedAt = e.CompletedAt,
        ProducedQuantity = e.ProducedQuantity,
        RejectedQuantity = e.RejectedQuantity,
        Status = e.Status,
        Remarks = e.Remarks,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt,
        CreatedBy = e.CreatedBy
    };
}
