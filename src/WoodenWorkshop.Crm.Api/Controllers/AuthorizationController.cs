using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Models.Http;
using WoodenWorkshop.Crm.Api.Options;
using IAuthorizationService = WoodenWorkshop.Crm.Api.Services.Abstractions.IAuthorizationService;

namespace WoodenWorkshop.Crm.Api.Controllers;

[AllowAnonymous, Route("api/auth")]
public class AuthorizationController : ControllerBase
{
    private const string RefreshTokenCookieName = "ww-refresh-token";

    private readonly IAuthorizationService _authorizationService;

    private readonly IUserSessionService _userSessionService;
    
    private readonly IOptionsSnapshot<Security> _securityOptions;

    private string? ClientIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();

    private int AccessTokenTtl => _securityOptions.Value.AccessTokenTtlSeconds;


    public AuthorizationController(
        IAuthorizationService authorizationService,
        IUserSessionService userSessionService,
        IOptionsSnapshot<Security> securityOptions
    )
    {
        _authorizationService = authorizationService;
        _userSessionService = userSessionService;
        _securityOptions = securityOptions;
    }


    [HttpPost("")]
    public async Task<ActionResult<AuthorizationResponse>> AuthorizeAsync(
        [FromBody] UserCredentialsDto userCredential
    )
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(userCredential, ClientIpAddress);
        AddRefreshTokenCookie(authorizationResult.RefreshToken);
        var authorizationResponse = new AuthorizationResponse(
            authorizationResult.AccessToken.Token,
            authorizationResult.RefreshToken.Token,
            AccessTokenTtl
        );
        return Ok(authorizationResponse);
    }

    [HttpPut("refresh")]
    public async Task<ActionResult<AuthorizationResponse>> RefreshTokenAsync(
        [FromBody] RefreshTokenDto refreshTokenDto
    )
    {
        var refreshToken = refreshTokenDto.RefreshToken ?? GetRefreshTokenFromCookies();
        if (refreshToken is null)
        {
            throw new BadHttpRequestException("Токен не был найден ни в теле запроса, ни в кухах.");
        }

        var refreshResult = await _authorizationService.RefreshSessionAsync(refreshToken);
        AddRefreshTokenCookie(refreshResult.RefreshToken);

        var authorizationResponse = new AuthorizationResponse(
            refreshResult.AccessToken.Token,
            refreshResult.RefreshToken.Token,
            AccessTokenTtl
        );
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