using NovaERP.Application.Common.Models;
using NovaERP.Application.Features.AuditLogs.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IAuditLogService
{
    Task<PagedResult<AuditLogDto>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);
    Task<AuditLogDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLogDto>> GetByUserIdAsync(Guid userId);
}
