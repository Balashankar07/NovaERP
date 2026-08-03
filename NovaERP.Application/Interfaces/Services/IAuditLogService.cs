using NovaERP.Application.Features.AuditLogs.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetAllAsync();
    Task<AuditLogDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(Guid userId);
}
