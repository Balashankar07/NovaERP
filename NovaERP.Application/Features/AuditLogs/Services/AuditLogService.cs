using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.AuditLogs.DTOs;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.Application.Features.AuditLogs.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<AuditLogDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null)
    {
        var pagedResult = await _unitOfWork.AuditLogs.GetAllAsync(pageNumber, pageSize, search, sortBy, sortOrder);
        return new PagedResult<AuditLogDto>
        {
            Items = pagedResult.Items.Select(MapToDto).OrderByDescending(x => x.Timestamp),
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id)
    {
        var log = await _unitOfWork.AuditLogs.GetByIdAsync(id);
        return log == null ? null : MapToDto(log);
    }

    public async Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(Guid userId)
    {
        var allLogs = await _unitOfWork.AuditLogs.GetAllAsync(1, int.MaxValue);
        var userLogs = allLogs.Items.Where(l => l.UserId == userId).OrderByDescending(x => x.Timestamp);
        return userLogs.Select(MapToDto);
    }

    private static AuditLogDto MapToDto(NovaERP.Domain.Entities.AuditLog log)
    {
        return new AuditLogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = log.Action,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            OldValues = log.OldValues,
            NewValues = log.NewValues,
            IpAddress = log.IpAddress,
            Timestamp = log.Timestamp
        };
    }
}
