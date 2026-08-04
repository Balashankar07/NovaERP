using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.PurchaseOrders.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;
using NovaERP.Domain.Enums;

namespace NovaERP.Infrastructure.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public PurchaseOrderService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var pagedPOs = await _unitOfWork.PurchaseOrders.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        
        return new PagedResult<PurchaseOrderDto>
        {
            Items = pagedPOs.Items.Select(MapToDto).ToList(),
            TotalCount = pagedPOs.TotalCount,
            PageNumber = pagedPOs.PageNumber,
            PageSize = pagedPOs.PageSize
        };
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(Guid id)
    {
        var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(id);
        return po == null ? null : MapToDto(po);
    }

    public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);
        if (supplier == null || !supplier.IsActive)
            throw new Exception("Supplier is invalid or inactive.");

        if (dto.ExpectedDeliveryDate < DateTime.UtcNow.Date)
            throw new Exception("Expected delivery date cannot be in the past.");

        var po = new PurchaseOrder
        {
            PONumber = await _unitOfWork.PurchaseOrders.GeneratePONumberAsync(),
            SupplierId = dto.SupplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Status = PurchaseOrderStatus.Draft,
            Currency = dto.Currency,
            Remarks = dto.Remarks,
            IsActive = true
        };

        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        if (productIds.Count != dto.Items.Count)
            throw new Exception("Duplicate products are not allowed in a Purchase Order.");

        foreach (var itemDto in dto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                throw new Exception($"Product with ID {itemDto.ProductId} not found.");

            var item = new PurchaseOrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                Discount = itemDto.Discount,
                Tax = itemDto.Tax,
                Remarks = itemDto.Remarks,
                LineTotal = (itemDto.Quantity * itemDto.UnitPrice) - itemDto.Discount + itemDto.Tax
            };
            po.Items.Add(item);
        }

        CalculateTotals(po);

        await _unitOfWork.PurchaseOrders.AddAsync(po);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Create", "PurchaseOrder", po.Id.ToString(), newValues: $"PONumber: {po.PONumber}, SupplierId: {po.SupplierId}, Total: {po.TotalAmount}");

        return MapToDto(po);
    }

    public async Task<PurchaseOrderDto?> UpdateAsync(Guid id, UpdatePurchaseOrderDto dto)
    {
        var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(id);
        if (po == null) return null;

        if (po.Status != PurchaseOrderStatus.Draft && po.Status != PurchaseOrderStatus.Rejected)
            throw new Exception("Only Draft or Rejected orders can be edited.");

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId);
        if (supplier == null || !supplier.IsActive)
            throw new Exception("Supplier is invalid or inactive.");

        po.SupplierId = dto.SupplierId;
        po.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        po.Currency = dto.Currency;
        po.Remarks = dto.Remarks;
        po.IsActive = dto.IsActive;

        // Process items
        var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
        if (productIds.Count != dto.Items.Count)
            throw new Exception("Duplicate products are not allowed in a Purchase Order.");

        // Remove deleted items
        var incomingItemIds = dto.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToList();
        var itemsToRemove = po.Items.Where(i => !incomingItemIds.Contains(i.Id)).ToList();
        foreach (var item in itemsToRemove)
        {
            po.Items.Remove(item);
        }

        // Add or update items
        foreach (var itemDto in dto.Items)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(itemDto.ProductId);
            if (product == null)
                throw new Exception($"Product with ID {itemDto.ProductId} not found.");

            if (itemDto.Id.HasValue)
            {
                var existingItem = po.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                if (existingItem != null)
                {
                    existingItem.ProductId = itemDto.ProductId;
                    existingItem.Quantity = itemDto.Quantity;
                    existingItem.UnitPrice = itemDto.UnitPrice;
                    existingItem.Discount = itemDto.Discount;
                    existingItem.Tax = itemDto.Tax;
                    existingItem.Remarks = itemDto.Remarks;
                    existingItem.LineTotal = (itemDto.Quantity * itemDto.UnitPrice) - itemDto.Discount + itemDto.Tax;
                }
            }
            else
            {
                var newItem = new PurchaseOrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    Discount = itemDto.Discount,
                    Tax = itemDto.Tax,
                    Remarks = itemDto.Remarks,
                    LineTotal = (itemDto.Quantity * itemDto.UnitPrice) - itemDto.Discount + itemDto.Tax
                };
                po.Items.Add(newItem);
            }
        }

        CalculateTotals(po);

        _unitOfWork.PurchaseOrders.Update(po);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Update", "PurchaseOrder", po.Id.ToString(), newValues: $"PONumber: {po.PONumber}, TotalAmount: {po.TotalAmount}");

        return MapToDto(po);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var po = await _unitOfWork.PurchaseOrders.GetPurchaseOrderWithItemsAsync(id);
        if (po == null) return false;

        if (po.Status != PurchaseOrderStatus.Draft)
            throw new Exception("Only Draft orders can be deleted.");

        _unitOfWork.PurchaseOrders.Delete(po);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "PurchaseOrder", po.Id.ToString(), oldValues: $"PONumber: {po.PONumber}");

        return true;
    }

    public async Task<PurchaseOrderDto?> SubmitAsync(Guid id)
    {
        var po = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
        if (po == null) return null;

        if (po.Status != PurchaseOrderStatus.Draft)
            throw new Exception("Only Draft orders can be submitted.");

        po.Status = PurchaseOrderStatus.PendingApproval;
        
        _unitOfWork.PurchaseOrders.Update(po);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "PurchaseOrder", po.Id.ToString(), oldValues: "Draft", newValues: "PendingApproval");

        return MapToDto(po);
    }

    public async Task<PurchaseOrderDto?> ApproveAsync(Guid id)
    {
        var po = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
        if (po == null) return null;

        if (po.Status != PurchaseOrderStatus.PendingApproval)
            throw new Exception("Only PendingApproval orders can be approved.");

        po.Status = PurchaseOrderStatus.Approved;
        
        _unitOfWork.PurchaseOrders.Update(po);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "PurchaseOrder", po.Id.ToString(), oldValues: "PendingApproval", newValues: "Approved");

        return MapToDto(po);
    }

    public async Task<PurchaseOrderDto?> RejectAsync(Guid id)
    {
        var po = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
        if (po == null) return null;

        if (po.Status != PurchaseOrderStatus.PendingApproval)
            throw new Exception("Only PendingApproval orders can be rejected.");

        po.Status = PurchaseOrderStatus.Rejected;
        
        _unitOfWork.PurchaseOrders.Update(po);
        await _unitOfWork.SaveChangesAsync();
        await _auditLogger.LogAsync("StatusChange", "PurchaseOrder", po.Id.ToString(), oldValues: "PendingApproval", newValues: "Rejected");

        return MapToDto(po);
    }

    private void CalculateTotals(PurchaseOrder po)
    {
        po.Subtotal = po.Items.Sum(i => i.Quantity * i.UnitPrice);
        po.DiscountAmount = po.Items.Sum(i => i.Discount);
        po.TaxAmount = po.Items.Sum(i => i.Tax);
        po.TotalAmount = po.Subtotal - po.DiscountAmount + po.TaxAmount;
    }

    private PurchaseOrderDto MapToDto(PurchaseOrder po)
    {
        return new PurchaseOrderDto
        {
            Id = po.Id,
            PONumber = po.PONumber,
            SupplierId = po.SupplierId,
            SupplierName = po.Supplier?.SupplierName ?? string.Empty,
            OrderDate = po.OrderDate,
            ExpectedDeliveryDate = po.ExpectedDeliveryDate,
            Status = po.Status.ToString(),
            Currency = po.Currency,
            Subtotal = po.Subtotal,
            TaxAmount = po.TaxAmount,
            DiscountAmount = po.DiscountAmount,
            TotalAmount = po.TotalAmount,
            Remarks = po.Remarks,
            IsActive = po.IsActive,
            CreatedAt = po.CreatedAt,
            UpdatedAt = po.UpdatedAt,
            Items = po.Items.Select(i => new PurchaseOrderItemDto
            {
                Id = i.Id,
                PurchaseOrderId = i.PurchaseOrderId,
                ProductId = i.ProductId,
                ProductCode = i.Product?.ProductCode ?? string.Empty,
                ProductName = i.Product?.Name ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                Tax = i.Tax,
                LineTotal = i.LineTotal,
                Remarks = i.Remarks
            }).ToList()
        };
    }
}
