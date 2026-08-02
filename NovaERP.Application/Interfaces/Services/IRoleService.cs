using NovaERP.Application.Features.Roles.DTOs;

namespace NovaERP.Application.Interfaces.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllAsync();

    Task<RoleDto?> GetByIdAsync(Guid id);

    Task<RoleDto> CreateAsync(CreateRoleDto dto);

    Task UpdateAsync(Guid id, UpdateRoleDto dto);

    Task DeleteAsync(Guid id);
}