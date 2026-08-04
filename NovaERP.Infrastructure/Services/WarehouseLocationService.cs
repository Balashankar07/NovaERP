using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.WarehouseLocations.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Services;

public class WarehouseLocationService : IWarehouseLocationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public WarehouseLocationService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<WarehouseLocationDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var result = await _unitOfWork.WarehouseLocations.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<WarehouseLocationDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<PagedResult<WarehouseLocationDto>> GetByWarehouseIdAsync(Guid warehouseId, int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var result = await _unitOfWork.WarehouseLocations.GetPagedByWarehouseIdAsync(warehouseId, pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<WarehouseLocationDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<WarehouseLocationDto?> GetByIdAsync(Guid id)
    {
        var location = await _unitOfWork.WarehouseLocations.GetByIdAsync(id);
        if (location != null && location.Warehouse == null)
        {
            location.Warehouse = await _unitOfWork.Warehouses.GetByIdAsync(location.WarehouseId);
        }
        return location == null ? null : MapToDto(location);
    }

    public async Task<WarehouseLocationDto> CreateAsync(CreateWarehouseLocationDto dto)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(dto.WarehouseId);
        if (warehouse == null)
            throw new Exception("Warehouse must exist before creating locations.");

        if (await _unitOfWork.WarehouseLocations.ExistsByCodeAsync(dto.WarehouseId, dto.LocationCode))
            throw new Exception("LocationCode must be unique within a warehouse.");

        if (await _unitOfWork.WarehouseLocations.ExistsByNameAsync(dto.WarehouseId, dto.LocationName))
            throw new Exception("LocationName must be unique within a warehouse.");

        var location = new WarehouseLocation
        {
            WarehouseId = dto.WarehouseId,
            LocationCode = dto.LocationCode,
            LocationName = dto.LocationName,
            Zone = dto.Zone,
            Rack = dto.Rack,
            Shelf = dto.Shelf,
            Bin = dto.Bin,
            Description = dto.Description,
            IsActive = warehouse.IsActive // Deactivated warehouse -> deactivated location
        };

        await _unitOfWork.WarehouseLocations.AddAsync(location);
        await _unitOfWork.SaveChangesAsync();

        location.Warehouse = warehouse;

        await _auditLogger.LogAsync("Create", "WarehouseLocation", location.Id.ToString(), newValues: $"LocationCode: {location.LocationCode}");

        return MapToDto(location);
    }

    public async Task<WarehouseLocationDto?> UpdateAsync(Guid id, UpdateWarehouseLocationDto dto)
    {
        var location = await _unitOfWork.WarehouseLocations.GetByIdAsync(id);
        if (location == null) return null;

        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(location.WarehouseId);

        if (location.LocationName != dto.LocationName)
        {
            if (await _unitOfWork.WarehouseLocations.ExistsByNameAsync(location.WarehouseId, dto.LocationName))
                throw new Exception("LocationName must be unique within a warehouse.");
        }

        location.LocationName = dto.LocationName;
        location.Zone = dto.Zone;
        location.Rack = dto.Rack;
        location.Shelf = dto.Shelf;
        location.Bin = dto.Bin;
        location.Description = dto.Description;
        
        if (warehouse != null && !warehouse.IsActive && dto.IsActive)
            throw new Exception("Cannot activate location when its warehouse is inactive.");

        location.IsActive = dto.IsActive;

        _unitOfWork.WarehouseLocations.Update(location);
        await _unitOfWork.SaveChangesAsync();

        location.Warehouse = warehouse;

        await _auditLogger.LogAsync("Update", "WarehouseLocation", location.Id.ToString(), newValues: $"LocationName: {location.LocationName}");

        return MapToDto(location);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var location = await _unitOfWork.WarehouseLocations.GetByIdAsync(id);
        if (location == null) return false;

        _unitOfWork.WarehouseLocations.Delete(location);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "WarehouseLocation", location.Id.ToString(), oldValues: $"LocationCode: {location.LocationCode}");

        return true;
    }

    private WarehouseLocationDto MapToDto(WarehouseLocation entity)
    {
        return new WarehouseLocationDto
        {
            Id = entity.Id,
            WarehouseId = entity.WarehouseId,
            WarehouseCode = entity.Warehouse?.WarehouseCode ?? string.Empty,
            WarehouseName = entity.Warehouse?.WarehouseName ?? string.Empty,
            LocationCode = entity.LocationCode,
            LocationName = entity.LocationName,
            Zone = entity.Zone,
            Rack = entity.Rack,
            Shelf = entity.Shelf,
            Bin = entity.Bin,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
