using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Models.Http;
using WoodenWorkshop.Crm.Api.Options;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Controllers;

[AllowAnonymous, Route("api/auth")]
public class AuthorizationController : ControllerBase
{
    private const string RefreshTokenCookieName = "Refresh-Token";

    private readonly IUserSessionService _userSessionService;

    private readonly IUsersService _usersService;

    private readonly ITokenService _tokenService;

    private readonly IOptionsSnapshot<Security> _securityOptions;

    private string? ClientIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();


    public AuthorizationController(
        IUserSessionService userSessionService,
        IUsersService usersService,
        ITokenService tokenService,
        IOptionsSnapshot<Security> securityOptions
    )
    {
        _userSessionService = userSessionService;
        _usersService = usersService;
        _tokenService = tokenService;
        _securityOptions = securityOptions;
    }


    [HttpPost("")]
    public async Task<ActionResult<AuthorizationResponse>> AuthorizeAsync(
        [FromBody] UserCredentialsDto userCredential
    )
    {
        var user = await _usersService.GetUserDetailsAsync(userCredential.Username, userCredential.Password);
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToArray();
        var accessToken = _tokenService.BuildAccessToken(user, permissions);
        var refreshToken = _tokenService.BuildRefreshToken();

        await _userSessionService.SaveUserSession(
            new(
                refreshToken.Token,
                user.Id,
                ClientIpAddress ?? "UNKNOWN",
                userCredential.DeviceName ?? "UNKNOWN"
            )
        );

        AddRefreshTokenCookie(refreshToken);

        var authorizationResponse = new AuthorizationResponse(
            accessToken.Token,
            refreshToken.Token,
            "bearer",
            _securityOptions.Value.AccessTokenTtlSeconds
        );

        return Ok(authorizationResponse);
    }

    [HttpPut("refresh")]
    public async Task<ActionResult<AuthorizationResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto
    )
    {
        if (refreshTokenDto.RefreshToken is null && !Request.Cookies.ContainsKey(RefreshTokenCookieName))
        {
            throw new BadHttpRequestException("Токен не был найден ни в теле запроса, ни в кухах.");
        }
        var session = await _userSessionService.GetUserSession(
            refreshTokenDto.RefreshToken ?? Request.Cookies[RefreshTokenCookieName]
        );
        var user = await _usersService.GetUserDetailsAsync(session.UserId);
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToArray();
        var newAccessToken = _tokenService.BuildAccessToken(user, permissions);
        var newRefreshToken = _tokenService.BuildRefreshToken();

        await _userSessionService.ExpireUserSession(refreshTokenDto.RefreshToken);
        await _userSessionService.SaveUserSession(
            new(
                newRefreshToken.Token,
                user.Id,
                session.IpAddress,
                session.DeviceName
            )
        );

        AddRefreshTokenCookie(newRefreshToken);


        var authorizationResponse = new AuthorizationResponse(
            newAccessToken.Token,
            newRefreshToken.Token,
            "bearer",
            _securityOptions.Value.AccessTokenTtlSeconds
        );

        return Ok(authorizationResponse);
    }

    [HttpPut("expire")]
    public async Task<NoContentResult> ExpireRefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto
    )
    {
        if (refreshTokenDto.RefreshToken is null && !Request.Cookies.ContainsKey(RefreshTokenCookieName))
        {
            throw new BadHttpRequestException("Токен не был найден ни в теле запроса, ни в кухах.");
        }

        await _userSessionService.ExpireUserSession(
            refreshTokenDto.RefreshToken ?? Request.Cookies[RefreshTokenCookieName]
        );

        return NoContent();
    }

    private void AddRefreshTokenCookie(TokenInfo tokenInfo)
    {
        Response.Cookies.Append(
            RefreshTokenCookieName,
            tokenInfo.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Expires = tokenInfo.ExpireDate,
                SameSite = SameSiteMode.Strict,
            }
        );
    }
}