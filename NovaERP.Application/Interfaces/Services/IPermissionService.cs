using NovaERP.Application.Features.Permissions.DTOs;

namespace NovaERP.Application.Interfaces.Services
{
    public interface IPermissionService
    {
        Task<IEnumerable<PermissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(Guid roleId);
        Task AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds);
    }
}
