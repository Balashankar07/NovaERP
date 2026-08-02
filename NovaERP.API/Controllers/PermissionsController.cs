using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.Application.Features.Permissions.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return Ok(permissions);
    }

    [HttpGet("role/{roleId}")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
        return Ok(permissions);
    }

    [HttpPost("role/{roleId}")]
    public async Task<IActionResult> AssignPermissionsToRole(Guid roleId, [FromBody] RolePermissionDto dto)
    {
        if (roleId != dto.RoleId)
        {
            return BadRequest("Role ID in URL must match Role ID in body.");
        }

        try
        {
            await _permissionService.AssignPermissionsToRoleAsync(roleId, dto.PermissionIds);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
