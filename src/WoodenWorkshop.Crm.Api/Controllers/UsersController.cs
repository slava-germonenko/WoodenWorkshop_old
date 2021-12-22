using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class UsersController : UserAwareController
{
    private readonly IUserRolesService _userRolesService;


    public UsersController(IUserRolesService userRolesService)
    {
        _userRolesService = userRolesService;
    }


    [HttpGet("current")]
    public ActionResult<UserDto> GetCurrentUser()
    {
        return new UserDto(CurrentUserId);
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
}