using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Extensions;
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

    private int AccessTokenTtl => _securityOptions.Value.AccessTokenTtlSeconds;


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
        var (accessToken, refreshToken) = _tokenService.BuildAccessAndRefreshTokens(user);

        await _userSessionService.SaveSessionAsync(new Session
        {
            DeviceName = userCredential.DeviceName,
            ExpireDate = refreshToken.ExpireDate,
            IpAddress = ClientIpAddress,
            RefreshToken = refreshToken.Token,
            UserId = user.Id,
        });
        AddRefreshTokenCookie(refreshToken);

        var authorizationResponse = new AuthorizationResponse(accessToken.Token, refreshToken.Token, AccessTokenTtl);
        return Ok(authorizationResponse);
    }

    [HttpPut("refresh")]
    public async Task<ActionResult<AuthorizationResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto
    )
    {
        var token = refreshTokenDto.RefreshToken ?? GetRefreshTokenFromCookies();
        if (token is null)
        {
            throw new BadHttpRequestException("Токен не был найден ни в теле запроса, ни в кухах.");
        }

        var newRefreshToken = _tokenService.BuildRefreshToken();
        var session = await _userSessionService.RefreshSessionAsync(token, newRefreshToken.Token);
        var user = await _usersService.GetUserDetailsAsync(session.UserId);
        var (accessToken, _) = _tokenService.BuildAccessAndRefreshTokens(user);

        AddRefreshTokenCookie(newRefreshToken);

        var authorizationResponse = new AuthorizationResponse(accessToken.Token, newRefreshToken.Token, AccessTokenTtl);
        return Ok(authorizationResponse);
    }

    [HttpPut("expire")]
    public async Task<NoContentResult> ExpireRefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto
    )
    {
        var token = refreshTokenDto.RefreshToken ?? GetRefreshTokenFromCookies();
        if (token is null)
        {
            throw new BadHttpRequestException("Токен не был найден ни в теле запроса, ни в кухах.");
        }

        await _userSessionService.ExpireSessionAsync(token);

        return NoContent();
    }

    [HttpGet("sessions/current")]
    public async Task<ActionResult<Session>> GetCurrentSession()
    {
        var refreshToken = GetRefreshTokenFromCookies();
        if (refreshToken is null)
        {
            throw new BadHttpRequestException("Токен не был найден.");
        }

        var session = await _userSessionService.GetSessionAsync(refreshToken);
        return Ok(session);
    }

    [HttpDelete("sessions/{sessionId:guid}")]
    public async Task<NoContentResult> RemoveSessionAsync(Guid sessionId)
    {
        await _userSessionService.ExpireSessionAsync(sessionId);
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

    private string? GetRefreshTokenFromCookies() => Request.Cookies[RefreshTokenCookieName];
}