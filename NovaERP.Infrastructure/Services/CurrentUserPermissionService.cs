using Microsoft.Extensions.Logging;
using NovaERP.Application.Interfaces.Repositories;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.Infrastructure.Services;

/// <summary>
/// Resolves whether the current user's role carries a named permission
/// by querying the existing RolePermission and Permission tables.
/// </summary>
public class CurrentUserPermissionService : ICurrentUserPermissionService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CurrentUserPermissionService> _logger;

    public CurrentUserPermissionService(
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CurrentUserPermissionService> logger)
    {
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> HasPermissionAsync(string permissionName)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            _logger.LogDebug("HasPermissionAsync: user is not authenticated.");
            return false;
        }

        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("HasPermissionAsync: UserId claim is empty.");
            return false;
        }

        // 1. Resolve the user to get their RoleId
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
        {
            _logger.LogWarning("HasPermissionAsync: User {UserId} not found.", userId);
            return false;
        }

        var roleId = user.RoleId;

        // 2. Load all RolePermissions for that role
        var allRolePermissions = await _unitOfWork.RolePermissions.GetAllAsync();
        var permissionIds = allRolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToHashSet();

        if (permissionIds.Count == 0)
        {
            _logger.LogDebug(
                "HasPermissionAsync: Role {RoleId} has no permissions assigned.", roleId);
            return false;
        }

        // 3. Load all permissions and check by name
        var allPermissions = await _unitOfWork.Permissions.GetAllAsync();
        var hasPermission = allPermissions
            .Any(p => permissionIds.Contains(p.Id)
                   && string.Equals(p.Name, permissionName, StringComparison.OrdinalIgnoreCase));

        _logger.LogDebug(
            "HasPermissionAsync: User {UserId} / Role {RoleId} / Permission '{Permission}' → {Result}",
            userId, roleId, permissionName, hasPermission);

        return hasPermission;
    }
}
