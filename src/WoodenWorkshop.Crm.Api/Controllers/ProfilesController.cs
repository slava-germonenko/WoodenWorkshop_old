using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Extensions;

namespace WoodenWorkshop.Crm.Api.Controllers;

[Authorize, ApiController, Route("api/[controller]")]
public class ProfilesController : UserAwareController
{
    private readonly IUsersService _usersService;


    public ProfilesController(IUsersService usersService)
    {
        _usersService = usersService;
    }


    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ProfileDto>> GetProfileAsync(Guid userId)
    {
        var user = await _usersService.GetUserDetailsAsync(userId);
        return Ok((ProfileDto) user);
    }

    [HttpPatch("")]
    public async Task<ActionResult<ProfileDto>> UpdateProfileAsync(
        [FromBody] ProfileDto profileDto
    )
    {
        if (CurrentUserId != profileDto.UserId)
        {
            throw new UnauthorizedException("Вы не может обновить чужой профиль.");
        }

        var user = await _usersService.GetUserDetailsAsync(profileDto.UserId);
        user.UpdateFromProfile(profileDto);
        await _usersService.UpdateUserDetailsAsync(user);

        return Ok(profileDto);
    }
}