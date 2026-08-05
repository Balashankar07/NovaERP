using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Shipments.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IShipmentService
{
    Task<PagedResult<ShipmentDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<ShipmentDto?> GetByIdAsync(Guid id);
    Task<ShipmentDto> CreateAsync(CreateShipmentDto dto, Guid currentUserId);
    Task UpdateAsync(Guid id, UpdateShipmentDto dto, Guid currentUserId);
    Task DeleteAsync(Guid id, Guid currentUserId);
    Task DispatchAsync(Guid id, Guid currentUserId);
    Task DeliverAsync(Guid id, DeliverShipmentDto dto, Guid currentUserId);
    Task CancelAsync(Guid id, Guid currentUserId);
}
