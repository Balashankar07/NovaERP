using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.Warehouses.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;
using NovaERP.Domain.Entities;

namespace NovaERP.Infrastructure.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public WarehouseService(IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<PagedResult<WarehouseDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var result = await _unitOfWork.Warehouses.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<WarehouseDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };
    }

    public async Task<WarehouseDto?> GetByIdAsync(Guid id)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
        return warehouse == null ? null : MapToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
    {
        if (await _unitOfWork.Warehouses.ExistsByCodeAsync(dto.WarehouseCode))
            throw new Exception("WarehouseCode must be unique.");

        if (dto.IsDefault)
        {
            if (await _unitOfWork.Warehouses.HasDefaultWarehouseAsync())
                throw new Exception("Only one default warehouse is allowed.");
        }
        else
        {
            if (!await _unitOfWork.Warehouses.HasDefaultWarehouseAsync())
                dto.IsDefault = true; // First warehouse must be default
        }

        var warehouse = new Warehouse
        {
            WarehouseCode = dto.WarehouseCode,
            WarehouseName = dto.WarehouseName,
            Description = dto.Description,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            ManagerName = dto.ManagerName,
            Phone = dto.Phone,
            Email = dto.Email,
            IsDefault = dto.IsDefault,
            IsActive = true
        };

        await _unitOfWork.Warehouses.AddAsync(warehouse);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Create", "Warehouse", warehouse.Id.ToString(), newValues: $"WarehouseCode: {warehouse.WarehouseCode}");

        return MapToDto(warehouse);
    }

    public async Task<WarehouseDto?> UpdateAsync(Guid id, UpdateWarehouseDto dto)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
        if (warehouse == null) return null;

        if (dto.IsDefault && !warehouse.IsDefault)
        {
            if (await _unitOfWork.Warehouses.HasDefaultWarehouseAsync(warehouse.Id))
                throw new Exception("Only one default warehouse is allowed.");
        }
        else if (!dto.IsDefault && warehouse.IsDefault)
        {
            throw new Exception("Cannot un-default a warehouse. Set another warehouse as default first (not supported directly) or leave this one as default.");
        }

        bool deactivated = warehouse.IsActive && !dto.IsActive;

        warehouse.WarehouseName = dto.WarehouseName;
        warehouse.Description = dto.Description;
        warehouse.Address = dto.Address;
        warehouse.City = dto.City;
        warehouse.State = dto.State;
        warehouse.Country = dto.Country;
        warehouse.PostalCode = dto.PostalCode;
        warehouse.ManagerName = dto.ManagerName;
        warehouse.Phone = dto.Phone;
        warehouse.Email = dto.Email;
        warehouse.IsDefault = dto.IsDefault;
        warehouse.IsActive = dto.IsActive;

        _unitOfWork.Warehouses.Update(warehouse);
        
        if (deactivated)
        {
            var locations = await _unitOfWork.WarehouseLocations.GetByWarehouseIdAsync(warehouse.Id);
            foreach(var loc in locations)
            {
                loc.IsActive = false;
                _unitOfWork.WarehouseLocations.Update(loc);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Update", "Warehouse", warehouse.Id.ToString(), newValues: $"WarehouseName: {warehouse.WarehouseName}");

        return MapToDto(warehouse);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
        if (warehouse == null) return false;

        if (warehouse.IsDefault)
            throw new Exception("Default warehouse cannot be deleted.");

        if (await _unitOfWork.WarehouseLocations.AnyLocationsInWarehouseAsync(id))
            throw new Exception("Warehouse with locations cannot be deleted.");

        _unitOfWork.Warehouses.Delete(warehouse);
        await _unitOfWork.SaveChangesAsync();

        await _auditLogger.LogAsync("Delete", "Warehouse", warehouse.Id.ToString(), oldValues: $"WarehouseCode: {warehouse.WarehouseCode}");

        return true;
    }

    private WarehouseDto MapToDto(Warehouse entity)
    {
        return new WarehouseDto
        {
            Id = entity.Id,
            WarehouseCode = entity.WarehouseCode,
            WarehouseName = entity.WarehouseName,
            Description = entity.Description,
            Address = entity.Address,
            City = entity.City,
            State = entity.State,
            Country = entity.Country,
            PostalCode = entity.PostalCode,
            ManagerName = entity.ManagerName,
            Phone = entity.Phone,
            Email = entity.Email,
            IsDefault = entity.IsDefault,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
