using NovaERP.Domain.Entities;
using NovaERP.Application.Common.Models;

namespace NovaERP.Application.Interfaces.Repositories;

public interface IGoodsReceiptRepository : IRepository<GoodsReceipt>
{
    Task<GoodsReceipt?> GetGoodsReceiptWithItemsAsync(Guid id);
    Task<List<GoodsReceipt>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId);
    Task<string> GenerateGRNNumberAsync();
}
