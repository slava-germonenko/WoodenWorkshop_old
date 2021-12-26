using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.Roles.Models;
using WoodenWorkshop.Core.Roles.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[Authorize, ApiController, Route("api/[controller]"), RequirePermissions(Permissions.Admin)]
public class RolesController : ControllerBase
{
    private readonly IRolePermissionsService _rolePermissionsService;

    private readonly IRolesListService _rolesListService;

    private readonly IRolesService _rolesService;

    public RolesController(
        IRolePermissionsService rolePermissionsService,
        IRolesListService rolesListService,
        IRolesService rolesService
    )
    {
        _rolePermissionsService = rolePermissionsService;
        _rolesListService = rolesListService;
        _rolesService = rolesService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<RoleDto>>> GetRolesListAsync(
        [FromQuery] Page page,
        [FromQuery] OrderByQuery orderByQuery,
        [FromQuery] RolesFilter rolesFilter
    )
    {
        var roles = await _rolesListService.GetRolesListAsync(page, rolesFilter, orderByQuery);
        var roleDtos = new PagedCollection<RoleDto>(
            page,
            roles.Items.Select(r => (RoleDto) r).ToList(),
            roles.Total
        );

        return Ok(roleDtos);
    }

    [HttpPost("")]
    public async Task<ActionResult<RoleDto>> CreateRoleAsync(RoleDto role)
    {
        var newRole = await _rolesListService.AddRoleAsync(new Role
        {
            Name = role.Name,
        });

        return Ok((RoleDto) newRole);
    }

    [HttpPatch("")]
    public async Task<ActionResult<RoleDto>> UpdateRoleDetailsAsync(RoleDto role)
    {
        var updatedRole = await _rolesService.UpdateRoleAsync(new Role
        {
            Id = role.Id,
            Name = role.Name,
        });

        return Ok((RoleDto) role);
    }

    [HttpDelete("{roleId:guid}")]
    public async Task<NoContentResult> RemoveRoleAsync(Guid roleId)
    {
        await _rolesListService.RemoveRoleAsync(roleId);
        return NoContent();
    }

    [HttpGet("{roleId:guid}/permissions")]
    public async Task<ActionResult<PermissionsDto>> AddRolePermissionsAsync(Guid roleId)
    {
        var role = await _rolesService.GetRoleAsync(roleId);
        var permissions = role.Permissions.Select(p => p.Name).ToArray();
        return Ok(new PermissionsDto(permissions));
    }

    [HttpPut("{roleId:guid}/permissions")]
    public async Task<NoContentResult> SetRolePermissions(Guid roleId, [FromBody] PermissionsDto permissions)
    {
        await _rolePermissionsService.SetRolePermissionsAsync(roleId, permissions.Permissions);
        return NoContent();
    }
}