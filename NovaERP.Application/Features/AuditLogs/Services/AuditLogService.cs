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

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var logs = await _unitOfWork.AuditLogs.GetAllAsync();
        return logs.Select(MapToDto).OrderByDescending(x => x.Timestamp);
    }

    public async Task<AuditLogDto?> GetByIdAsync(Guid id)
    {
        var log = await _unitOfWork.AuditLogs.GetByIdAsync(id);
        return log == null ? null : MapToDto(log);
    }

    public async Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(Guid userId)
    {
        var allLogs = await _unitOfWork.AuditLogs.GetAllAsync();
        var userLogs = allLogs.Where(l => l.UserId == userId).OrderByDescending(x => x.Timestamp);
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
