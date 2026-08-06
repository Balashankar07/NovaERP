using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Features.Reports.DTOs;
using NovaERP.Application.Features.Reports.Interfaces;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories.Reports;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var totalProducts = await _context.Products.CountAsync(cancellationToken);
        var totalSuppliers = await _context.Suppliers.CountAsync(cancellationToken);
        var totalWarehouses = await _context.Warehouses.CountAsync(cancellationToken);

        var totalInventoryValue = await _context.Inventories
            .Join(_context.Products, i => i.ProductId, p => p.Id, (i, p) => i.QuantityOnHand * p.CostPrice)
            .SumAsync(cancellationToken);

        var openPurchaseOrders = await _context.PurchaseOrders
            .CountAsync(po => po.Status == NovaERP.Domain.Enums.PurchaseOrderStatus.Draft || po.Status == NovaERP.Domain.Enums.PurchaseOrderStatus.PendingApproval || po.Status == NovaERP.Domain.Enums.PurchaseOrderStatus.Approved, cancellationToken);

        var completedProductionOrders = await _context.ProductionOrders
            .CountAsync(po => po.Status == NovaERP.Domain.Enums.ProductionOrderStatus.Completed, cancellationToken);

        var pendingQualityInspections = await _context.QualityInspections
            .CountAsync(qi => qi.Status == NovaERP.Domain.Enums.QualityInspectionStatus.Draft || qi.Status == NovaERP.Domain.Enums.QualityInspectionStatus.InProgress, cancellationToken);

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        
        var salesThisMonth = await _context.SalesOrders
            .Where(so => so.OrderDate.Month == currentMonth && so.OrderDate.Year == currentYear)
            .SumAsync(so => so.TotalAmount, cancellationToken);

        var shipmentsPending = await _context.Shipments
            .CountAsync(s => s.Status == NovaERP.Domain.Enums.ShipmentStatus.Pending, cancellationToken);

        var activeWarranties = await _context.Warranties
            .CountAsync(w => w.Status == NovaERP.Domain.Enums.WarrantyStatus.Active, cancellationToken);

        var openWarrantyClaims = await _context.WarrantyClaims
            .CountAsync(wc => wc.Status == NovaERP.Domain.Enums.WarrantyClaimStatus.Pending || wc.Status == NovaERP.Domain.Enums.WarrantyClaimStatus.UnderReview, cancellationToken);

        return new DashboardSummaryDto
        {
            TotalProducts = totalProducts,
            TotalSuppliers = totalSuppliers,
            TotalWarehouses = totalWarehouses,
            TotalInventoryValue = totalInventoryValue,
            OpenPurchaseOrders = openPurchaseOrders,
            CompletedProductionOrders = completedProductionOrders,
            PendingQualityInspections = pendingQualityInspections,
            SalesThisMonth = salesThisMonth,
            ShipmentsPending = shipmentsPending,
            ActiveWarranties = activeWarranties,
            OpenWarrantyClaims = openWarrantyClaims
        };
    }

    public IQueryable<InventoryReportDto> GetInventoryReportQuery(Guid companyId)
    {
        return _context.Inventories.AsNoTracking()
            .Join(_context.Products, i => i.ProductId, p => p.Id, (i, p) => new InventoryReportDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductCode = p.ProductCode,
                QuantityOnHand = i.QuantityOnHand,
                MinStockLevel = i.MinimumLevel,
                MaxStockLevel = i.MaximumLevel,
                CostPrice = p.CostPrice,
                TotalValue = i.QuantityOnHand * p.CostPrice,
                LastRestockDate = i.LastStockUpdate
            });
    }

    public IQueryable<ProductionReportDto> GetProductionReportQuery(Guid companyId)
    {
        return _context.ProductionOrders.AsNoTracking()
            .Join(_context.Products, po => po.ProductId, p => p.Id, (po, p) => new ProductionReportDto
            {
                OrderId = po.Id,
                OrderNumber = po.ProductionOrderNumber,
                ProductName = p.Name,
                Quantity = po.PlannedQuantity,
                StartDate = po.PlannedStartDate ?? DateTime.MinValue,
                EndDate = po.PlannedEndDate ?? DateTime.MinValue,
                Status = po.Status.ToString()
            });
    }

    public IQueryable<SalesReportDto> GetSalesReportQuery(Guid companyId)
    {
        return _context.SalesOrders.AsNoTracking()
            .Select(so => new SalesReportDto
            {
                OrderId = so.Id,
                OrderNumber = so.OrderNumber,
                CustomerName = so.Distributor != null ? so.Distributor.CompanyName : "N/A",
                OrderDate = so.OrderDate,
                TotalAmount = so.TotalAmount,
                Status = so.Status.ToString()
            });
    }

    public IQueryable<WarrantyReportDto> GetWarrantyReportQuery(Guid companyId)
    {
        return _context.Warranties.AsNoTracking()
            .Select(w => new WarrantyReportDto
            {
                WarrantyId = w.Id,
                ProductName = w.Product != null ? w.Product.Name : "N/A",
                SerialNumber = w.SerialNumber,
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                Status = w.Status.ToString()
            });
    }

    public IQueryable<AuditReportDto> GetAuditReportQuery(Guid companyId)
    {
        return _context.AuditLogs
            .Select(a => new AuditReportDto
            {
                Id = a.Id,
                Action = a.Action,
                EntityName = a.EntityName,
                EntityId = a.EntityId,
                IpAddress = a.IpAddress,
                Timestamp = a.Timestamp,
                UserName = a.User != null ? a.User.FirstName + " " + a.User.LastName : string.Empty
            });
    }
}
