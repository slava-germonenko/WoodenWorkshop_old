using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.Users.Models;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Extensions;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class UsersController : UserAwareController
{
    private readonly IUserRolesService _userRolesService;

    private readonly IUsersListService _usersListService;
    
    private readonly IUsersService _usersService;

    private readonly ISessionsListService _sessionsListService;


    public UsersController(
        IUserRolesService userRolesService,
        IUsersListService usersListService,
        IUsersService usersService, 
        ISessionsListService sessionsListService
    )
    {
        _userRolesService = userRolesService;
        _usersListService = usersListService;
        _usersService = usersService;
        _sessionsListService = sessionsListService;
    }


    [HttpGet("current")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        return await GetUserAsync(CurrentUserId);
    }

    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<UserDto>> GetUserAsync(Guid userId)
    {
        var user = await _usersService.GetUserDetailsAsync(userId);
        return Ok((UserDto) user);
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<UserDto>>> GetUsersList(
        [FromQuery] Page page,
        [FromQuery] UsersFilter usersFilter,
        [FromQuery] OrderByQuery orderByQuery
    )
    {
        var users = await _usersListService.GetUsersListAsync(page, usersFilter, orderByQuery);
        var userDtos = new PagedCollection<UserDto>(
            users.Page,
            users.Items.Select((u) => (UserDto) u).ToList(),
            users.Total
        );
        return Ok(userDtos);
    }

    [HttpGet("{userId:guid}/permissions")]
    public async Task<ActionResult<PermissionsDto>> GetUserPermissionsAsync(Guid userId)
    {
        var roles = await _userRolesService.GetUserRolesAsync(userId);
        var permissionsDto = new PermissionsDto(
            roles.SelectMany(p => p.Permissions)
                .Select(p => p.Name)
                .ToList()
        );
        return Ok(permissionsDto);
    }

    [HttpGet("{userId:guid}/roles")]
    public async Task<ActionResult<ICollection<Role>>> GetUserRolesAsync(Guid userId)
    {
        var roles = await _userRolesService.GetUserRolesAsync(userId);
        return Ok(new UserRolesDto(roles));
    }

    [HttpPost(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<UserDto>> CreateUserAsync([FromBody] UserWithPasswordDto user)
    {
        var createdUser = await _usersListService.AddUserAsync((User) user);
        return Ok((UserDto) createdUser);
    }

    [HttpPatch(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<UserDto>> UpdateProfileAsync(
        [FromBody] UserDto userDto
    )
    {
        if (CurrentUserId != userDto.Id && CurrentUserPermissions.Contains(Permissions.Admin))
        {
            throw new UnauthorizedException("Вы не можете обновить чужой профиль.");
        }

        var user = await _usersService.GetUserDetailsAsync(userDto.Id);
        user.UpdateFromUserDto(userDto);
        await _usersService.UpdateUserDetailsAsync(user);

        return Ok(userDto);
    }

    [HttpPost("{userId:guid}/roles"), RequirePermissions(Permissions.Admin)]
    public async Task<NoContentResult> AddUserRoleAsync(
        [FromBody] UserRoleDto roleDto,
        [FromRoute] Guid userId
    )
    {
        await _userRolesService.AssignRoleToUserAsync(userId, roleDto.RoleId);
        return NoContent();
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}"), RequirePermissions(Permissions.Admin)]
    public async Task<NoContentResult> RemoveUserRoleAsync(Guid userId, Guid roleId)
    {
        await _userRolesService.UnassignRoleFromUserAsync(userId, roleId);
        return NoContent();
    }

    [HttpGet("{userId:guid}/sessions")]
    public async Task<ActionResult<ICollection<Session>>> GetUserSessions(Guid userId)
    {
        var sessions = await _sessionsListService.GetSessionsList(userId);
        return Ok(new { Sessions = sessions });
    }
}