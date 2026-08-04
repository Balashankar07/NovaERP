using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.GoodsReceipts.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;
using NovaERP.Domain.Enums;

namespace NovaERP.Infrastructure.Services;

public class GoodsReceiptService : IGoodsReceiptService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public GoodsReceiptService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<GoodsReceiptDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var pagedGRNs = await _unitOfWork.GoodsReceipts.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        
        return new PagedResult<GoodsReceiptDto>
        {
            Items = pagedGRNs.Items.Select(MapToDto).ToList(),
            TotalCount = pagedGRNs.TotalCount,
            PageNumber = pagedGRNs.PageNumber,
            PageSize = pagedGRNs.PageSize
        };
    }

    public async Task<GoodsReceiptDto?> GetByIdAsync(Guid id)
    {
        var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
        return grn == null ? null : MapToDto(grn);
    }

    public async Task<GoodsReceiptDto> CreateAsync(Guid currentUserId, CreateGoodsReceiptDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(dto.PurchaseOrderId);
            if (po == null)
                throw new Exception("Purchase Order not found.");

            if (po.Status != PurchaseOrderStatus.Approved)
                throw new Exception("Goods Receipt can only be created from an Approved Purchase Order.");

            var grn = new GoodsReceipt
            {
                GRNNumber = await _unitOfWork.GoodsReceipts.GenerateGRNNumberAsync(),
                PurchaseOrderId = po.Id,
                SupplierId = po.SupplierId,
                ReceiptDate = DateTime.UtcNow,
                Status = GoodsReceiptStatus.Draft,
                Remarks = dto.Remarks,
                ReceivedBy = currentUserId,
                IsActive = true
            };

            var purchaseOrderItemIds = dto.Items.Select(i => i.PurchaseOrderItemId).Distinct().ToList();
            if (purchaseOrderItemIds.Count != dto.Items.Count)
                throw new Exception("Duplicate purchase order items are not allowed in the same GRN.");

            var existingGRNs = await _unitOfWork.GoodsReceipts.GetByPurchaseOrderIdAsync(po.Id);
            var activeGRNs = existingGRNs.Where(g => g.Status != GoodsReceiptStatus.Cancelled).ToList();

            foreach (var itemDto in dto.Items)
            {
                var poItem = po.Items.FirstOrDefault(i => i.Id == itemDto.PurchaseOrderItemId);
                if (poItem == null)
                    throw new Exception($"Purchase Order Item with ID {itemDto.PurchaseOrderItemId} not found in the selected Purchase Order.");

                decimal alreadyReceived = activeGRNs
                    .SelectMany(g => g.Items)
                    .Where(i => i.PurchaseOrderItemId == poItem.Id)
                    .Sum(i => i.ReceivedQuantity);

                if (alreadyReceived + itemDto.ReceivedQuantity > poItem.Quantity)
                    throw new Exception($"Cumulative received quantity ({alreadyReceived + itemDto.ReceivedQuantity}) cannot exceed ordered quantity ({poItem.Quantity}) for product ID {poItem.ProductId}.");

                var item = new GoodsReceiptItem
                {
                    PurchaseOrderItemId = poItem.Id,
                    ProductId = poItem.ProductId,
                    OrderedQuantity = poItem.Quantity,
                    ReceivedQuantity = itemDto.ReceivedQuantity,
                    RejectedQuantity = itemDto.RejectedQuantity,
                    Remarks = itemDto.Remarks
                };
                grn.Items.Add(item);
            }

            await _unitOfWork.GoodsReceipts.AddAsync(grn);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogger.LogAsync("Create", "GoodsReceipt", grn.Id.ToString(), newValues: $"GRNNumber: {grn.GRNNumber}, PurchaseOrderId: {grn.PurchaseOrderId}");

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(grn);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GoodsReceiptDto?> UpdateAsync(Guid id, UpdateGoodsReceiptDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
            if (grn == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            if (grn.Status != GoodsReceiptStatus.Draft)
                throw new Exception("Only Draft GRNs can be edited.");

            var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(grn.PurchaseOrderId);
            if (po == null) throw new Exception("Associated Purchase Order not found.");

            grn.Remarks = dto.Remarks;
            grn.IsActive = dto.IsActive;

            var purchaseOrderItemIds = dto.Items.Select(i => i.PurchaseOrderItemId).Distinct().ToList();
            if (purchaseOrderItemIds.Count != dto.Items.Count)
                throw new Exception("Duplicate purchase order items are not allowed in the same GRN.");

            var incomingItemIds = dto.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToList();
            var itemsToRemove = grn.Items.Where(i => !incomingItemIds.Contains(i.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                grn.Items.Remove(item);
            }

            var existingGRNs = await _unitOfWork.GoodsReceipts.GetByPurchaseOrderIdAsync(po.Id);
            var activeOtherGRNs = existingGRNs.Where(g => g.Id != grn.Id && g.Status != GoodsReceiptStatus.Cancelled).ToList();

            foreach (var itemDto in dto.Items)
            {
                var poItem = po.Items.FirstOrDefault(i => i.Id == itemDto.PurchaseOrderItemId);
                if (poItem == null)
                    throw new Exception($"Purchase Order Item with ID {itemDto.PurchaseOrderItemId} not found in the selected Purchase Order.");

                decimal alreadyReceivedInOthers = activeOtherGRNs
                    .SelectMany(g => g.Items)
                    .Where(i => i.PurchaseOrderItemId == poItem.Id)
                    .Sum(i => i.ReceivedQuantity);

                if (alreadyReceivedInOthers + itemDto.ReceivedQuantity > poItem.Quantity)
                    throw new Exception($"Cumulative received quantity ({alreadyReceivedInOthers + itemDto.ReceivedQuantity}) cannot exceed ordered quantity ({poItem.Quantity}) for product ID {poItem.ProductId}.");

                if (itemDto.Id.HasValue)
                {
                    var existingItem = grn.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                    if (existingItem != null)
                    {
                        existingItem.PurchaseOrderItemId = poItem.Id;
                        existingItem.ProductId = poItem.ProductId;
                        existingItem.OrderedQuantity = poItem.Quantity;
                        existingItem.ReceivedQuantity = itemDto.ReceivedQuantity;
                        existingItem.RejectedQuantity = itemDto.RejectedQuantity;
                        existingItem.Remarks = itemDto.Remarks;
                    }
                }
                else
                {
                    var newItem = new GoodsReceiptItem
                    {
                        PurchaseOrderItemId = poItem.Id,
                        ProductId = poItem.ProductId,
                        OrderedQuantity = poItem.Quantity,
                        ReceivedQuantity = itemDto.ReceivedQuantity,
                        RejectedQuantity = itemDto.RejectedQuantity,
                        Remarks = itemDto.Remarks
                    };
                    grn.Items.Add(newItem);
                }
            }

            _unitOfWork.GoodsReceipts.Update(grn);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogger.LogAsync("Update", "GoodsReceipt", grn.Id.ToString(), newValues: $"GRNNumber: {grn.GRNNumber}");

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(grn);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
        if (grn == null) return false;

        if (grn.Status != GoodsReceiptStatus.Draft)
            throw new Exception("Only Draft GRNs can be deleted.");

        _unitOfWork.GoodsReceipts.Delete(grn);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "GoodsReceipt", grn.Id.ToString(), oldValues: $"GRNNumber: {grn.GRNNumber}");

        return true;
    }

    public async Task<GoodsReceiptDto?> ReceiveAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
            if (grn == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            if (grn.Status != GoodsReceiptStatus.Draft)
                throw new Exception("Only Draft GRNs can be received.");

            var existingGRNs = await _unitOfWork.GoodsReceipts.GetByPurchaseOrderIdAsync(grn.PurchaseOrderId);
            
            // To figure out if it is partially received, check if any item received quantity < ordered quantity for THIS GRN?
            // Actually, for GRN status, if *this* GRN itself doesn't fulfill everything, it might be partially received. 
            // Better yet, usually a GRN is PartiallyReceived if the *PO* is partially received. But the GRN enum has PartiallyReceived.
            // Let's keep the existing logic: if this GRN's items' received < ordered, it's PartiallyReceived.
            bool isPartiallyReceived = false;
            foreach (var item in grn.Items)
            {
                if (item.ReceivedQuantity < item.OrderedQuantity)
                {
                    isPartiallyReceived = true;
                    break;
                }
            }

            grn.Status = isPartiallyReceived ? GoodsReceiptStatus.PartiallyReceived : GoodsReceiptStatus.Completed;

            _unitOfWork.GoodsReceipts.Update(grn);
            await _unitOfWork.SaveChangesAsync();
            await _auditLogger.LogAsync("StatusChange", "GoodsReceipt", grn.Id.ToString(), oldValues: "Draft", newValues: grn.Status.ToString());

            if (grn.Status == GoodsReceiptStatus.Completed)
            {
                await CheckAndClosePurchaseOrderAsync(grn.PurchaseOrderId);
            }

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(grn);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GoodsReceiptDto?> CompleteAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
            if (grn == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return null;
            }

            if (grn.Status != GoodsReceiptStatus.PartiallyReceived)
                throw new Exception("Only PartiallyReceived GRNs can be completed manually.");

            grn.Status = GoodsReceiptStatus.Completed;

            _unitOfWork.GoodsReceipts.Update(grn);
            await _unitOfWork.SaveChangesAsync();
            await _auditLogger.LogAsync("StatusChange", "GoodsReceipt", grn.Id.ToString(), oldValues: "PartiallyReceived", newValues: "Completed");

            await CheckAndClosePurchaseOrderAsync(grn.PurchaseOrderId);

            await _unitOfWork.CommitTransactionAsync();
            return MapToDto(grn);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task CheckAndClosePurchaseOrderAsync(Guid poId)
    {
        var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(poId);
        if (po == null || po.Status == PurchaseOrderStatus.Closed) return;

        var existingGRNs = await _unitOfWork.GoodsReceipts.GetByPurchaseOrderIdAsync(poId);
        var activeGRNs = existingGRNs.Where(g => g.Status != GoodsReceiptStatus.Cancelled && g.Status != GoodsReceiptStatus.Draft).ToList();

        bool allItemsFullyReceived = true;
        foreach (var poItem in po.Items)
        {
            decimal totalReceived = activeGRNs
                .SelectMany(g => g.Items)
                .Where(i => i.PurchaseOrderItemId == poItem.Id)
                .Sum(i => i.ReceivedQuantity);

            if (totalReceived < poItem.Quantity)
            {
                allItemsFullyReceived = false;
                break;
            }
        }

        if (allItemsFullyReceived)
        {
            po.Status = PurchaseOrderStatus.Closed;
            _unitOfWork.PurchaseOrders.Update(po);
            await _unitOfWork.SaveChangesAsync();
            await _auditLogger.LogAsync("StatusChange", "PurchaseOrder", po.Id.ToString(), oldValues: "Approved", newValues: "Closed");
        }
    }

    public async Task<GoodsReceiptDto?> CancelAsync(Guid id)
    {
        var grn = await _unitOfWork.GoodsReceipts.GetGoodsReceiptWithItemsAsync(id);
        if (grn == null) return null;

        if (grn.Status != GoodsReceiptStatus.Draft)
            throw new Exception("Only Draft GRNs can be cancelled.");

        grn.Status = GoodsReceiptStatus.Cancelled;

        _unitOfWork.GoodsReceipts.Update(grn);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "GoodsReceipt", grn.Id.ToString(), oldValues: "Draft", newValues: "Cancelled");

        return MapToDto(grn);
    }

    private GoodsReceiptDto MapToDto(GoodsReceipt grn)
    {
        return new GoodsReceiptDto
        {
            Id = grn.Id,
            GRNNumber = grn.GRNNumber,
            PurchaseOrderId = grn.PurchaseOrderId,
            PurchaseOrderNumber = grn.PurchaseOrder?.PONumber ?? string.Empty,
            SupplierId = grn.SupplierId,
            SupplierName = grn.Supplier?.SupplierName ?? string.Empty,
            ReceiptDate = grn.ReceiptDate,
            Status = grn.Status.ToString(),
            Remarks = grn.Remarks,
            ReceivedBy = grn.ReceivedBy,
            IsActive = grn.IsActive,
            CreatedAt = grn.CreatedAt,
            UpdatedAt = grn.UpdatedAt,
            Items = grn.Items.Select(i => new GoodsReceiptItemDto
            {
                Id = i.Id,
                GoodsReceiptId = i.GoodsReceiptId,
                PurchaseOrderItemId = i.PurchaseOrderItemId,
                ProductId = i.ProductId,
                ProductCode = i.Product?.ProductCode ?? string.Empty,
                ProductName = i.Product?.Name ?? string.Empty,
                OrderedQuantity = i.OrderedQuantity,
                ReceivedQuantity = i.ReceivedQuantity,
                RejectedQuantity = i.RejectedQuantity,
                Remarks = i.Remarks
            }).ToList()
        };
    }
}
