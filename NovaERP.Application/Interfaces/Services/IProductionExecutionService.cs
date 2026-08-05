using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.ProductionExecutions.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IProductionExecutionService
{
    Task<PagedResult<ProductionExecutionDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<ProductionExecutionDto?> GetByIdAsync(Guid id);
    Task<ProductionExecutionDto> CreateAsync(CreateProductionExecutionDto dto, Guid? currentUserId);
    Task<ProductionExecutionDto> UpdateAsync(Guid id, UpdateProductionExecutionDto dto, Guid? currentUserId);
    Task<bool> DeleteAsync(Guid id, Guid? currentUserId);

    Task<ProductionExecutionDto> StartAsync(Guid id, Guid? currentUserId);
    Task<ProductionExecutionDto> ConsumeMaterialsAsync(Guid id, Guid? currentUserId);
    Task<ProductionExecutionDto> CompleteAsync(Guid id, CompleteProductionExecutionDto dto, Guid? currentUserId);
    Task<ProductionExecutionDto> CancelAsync(Guid id, string reason, Guid? currentUserId);
}
