using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovaERP.API.Authorization;
using NovaERP.Application.Features.Roles.DTOs;
using NovaERP.Application.Interfaces.Services;

namespace NovaERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [HasPermission("Roles.View")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _roleService.GetAllAsync());
    }

    [HttpGet("{id}")]
    [HasPermission("Roles.View")]
    public async Task<IActionResult> Get(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);

        if (role == null)
            return NotFound();

        return Ok(role);
    }

    [HttpPost]
    [HasPermission("Roles.Create")]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        var role = await _roleService.CreateAsync(dto);

        return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    [HasPermission("Roles.Update")]
    public async Task<IActionResult> Update(Guid id, UpdateRoleDto dto)
    {
        await _roleService.UpdateAsync(id, dto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [HasPermission("Roles.Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _roleService.DeleteAsync(id);

        return NoContent();
    }
}