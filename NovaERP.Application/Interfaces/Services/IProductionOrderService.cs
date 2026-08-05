using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductionOrders.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IProductionOrderService
{
    Task<PagedResult<ProductionOrderDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<ProductionOrderDto?> GetByIdAsync(Guid id);
    Task<ProductionOrderDto> CreateAsync(CreateProductionOrderDto dto, Guid? currentUserId);
    Task<ProductionOrderDto> UpdateAsync(Guid id, UpdateProductionOrderDto dto, Guid? currentUserId);
    Task<bool> DeleteAsync(Guid id, Guid? currentUserId);
    Task<ProductionOrderDto> ReleaseAsync(Guid id, Guid? currentUserId);
    Task<ProductionOrderDto> StartAsync(Guid id, decimal startedQuantity, Guid? currentUserId);
    Task<ProductionOrderDto> CompleteAsync(Guid id, decimal completedQuantity, decimal rejectedQuantity, Guid? currentUserId);
    Task<ProductionOrderDto> CancelAsync(Guid id, string reason, Guid? currentUserId);
}
