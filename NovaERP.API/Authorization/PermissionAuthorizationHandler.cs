using Microsoft.AspNetCore.Authorization;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Authorization;

/// <summary>
/// Handles <see cref="PermissionRequirement"/> by querying
/// <see cref="ICurrentUserPermissionService"/> for the current user's permissions.
/// </summary>
public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUserPermissionService _permissionService;

    public PermissionAuthorizationHandler(ICurrentUserPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var hasPermission = await _permissionService.HasPermissionAsync(requirement.PermissionName);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
