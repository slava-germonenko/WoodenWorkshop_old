using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.Users.Models;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Extensions;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class UsersController : UserAwareController
{
    private readonly IUserRolesService _userRolesService;

    private readonly IUsersListService _usersListService;
    
    private readonly IUsersService _usersService;


    public UsersController(
        IUserRolesService userRolesService,
        IUsersListService usersListService,
        IUsersService usersService
    )
    {
        _userRolesService = userRolesService;
        _usersListService = usersListService;
        _usersService = usersService;
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
        [FromQuery] OrderByQuery orderBy
    )
    {
        var users = await _usersListService.GetUsersListAsync(page, usersFilter, orderBy);
        var userDtos = new PagedCollection<UserDto>(
            users.Page,
            users.Items.Select((u) => (UserDto) u).ToList(),
            users.Total
        );
        return Ok(userDtos);
    }

    [HttpGet("{userId:guid}/permissions")]
    public async Task<ActionResult<UserPermissionsDto>> GetUserPermissionsAsync(Guid userId)
    {
        var roles = await _userRolesService.GetUserRolesAsync(userId);
        var permissionsDto = new UserPermissionsDto(
            roles.SelectMany(p => p.Permissions)
                .Select(p => p.Name)
                .ToList()
        );
        return Ok(permissionsDto);
    }
    
    [HttpPatch("")]
    public async Task<ActionResult<UserDto>> UpdateProfileAsync(
        [FromBody] UserDto userDto
    )
    {
        if (CurrentUserId != userDto.Id && CurrentUserPermissions.Contains(Permissions.Admin))
        {
            throw new UnauthorizedException("Вы не может обновить чужой профиль.");
        }

        var user = await _usersService.GetUserDetailsAsync(userDto.Id);
        user.UpdateFromUserDto(userDto);
        await _usersService.UpdateUserDetailsAsync(user);

        return Ok(userDto);
    }
}