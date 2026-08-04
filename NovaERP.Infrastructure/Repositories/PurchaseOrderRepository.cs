using Microsoft.EntityFrameworkCore;
using NovaERP.Application.Common.Models;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Domain.Entities;
using NovaERP.Infrastructure.Persistence.Context;

namespace NovaERP.Infrastructure.Repositories;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<PagedResult<PurchaseOrder>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.PONumber.Contains(search) || 
                                     (x.Supplier != null && x.Supplier.SupplierName.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            bool isDesc = sortOrder?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;
            query = sortBy.ToLower() switch
            {
                "ponumber" => isDesc ? query.OrderByDescending(x => x.PONumber) : query.OrderBy(x => x.PONumber),
                "orderdate" => isDesc ? query.OrderByDescending(x => x.OrderDate) : query.OrderBy(x => x.OrderDate),
                "status" => isDesc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
                "totalamount" => isDesc ? query.OrderByDescending(x => x.TotalAmount) : query.OrderBy(x => x.TotalAmount),
                "suppliername" => isDesc ? query.OrderByDescending(x => x.Supplier!.SupplierName) : query.OrderBy(x => x.Supplier!.SupplierName),
                "createdat" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt)
            };
        }
        else 
        {
            query = query.OrderByDescending(x => x.CreatedAt);
        }

        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;

        var totalCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(query);
        var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(query.Skip((pageNumber - 1) * pageSize).Take(pageSize));

        return new PagedResult<PurchaseOrder>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PurchaseOrder?> GetPurchaseOrderWithItemsAsync(Guid id)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(po => po.Id == id);
    }

    public async Task<string> GeneratePONumberAsync()
    {
        var prefix = "PO-";
        var datePart = DateTime.UtcNow.ToString("yyyyMM");
        
        var lastPO = await _context.PurchaseOrders
            .Where(po => po.PONumber.StartsWith(prefix + datePart))
            .OrderByDescending(po => po.PONumber)
            .FirstOrDefaultAsync();

        if (lastPO == null)
        {
            return $"{prefix}{datePart}-0001";
        }

        var sequenceStr = lastPO.PONumber.Substring(lastPO.PONumber.LastIndexOf('-') + 1);
        if (int.TryParse(sequenceStr, out int sequence))
        {
            return $"{prefix}{datePart}-{(sequence + 1):D4}";
        }

        return $"{prefix}{datePart}-0001";
    }
}
