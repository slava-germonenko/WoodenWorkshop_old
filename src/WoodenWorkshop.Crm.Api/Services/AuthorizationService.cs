using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IUserSessionService _userSessionService;

    private readonly ITokenService _tokenService;

    private readonly IUsersService _usersService;

    private readonly IUserRolesService _userRolesService;


    public AuthorizationService(
        IUserSessionService userSessionService,
        ITokenService tokenService,
        IUsersService usersService,
        IUserRolesService userRolesService
    )
    {
        _userSessionService = userSessionService;
        _tokenService = tokenService;
        _usersService = usersService;
        _userRolesService = userRolesService;
    }


    public async Task<AuthorizationResult> AuthorizeAsync(UserCredentialsDto userCredentials, string? ipAddress)
    {
        var user = await _usersService.GetUserDetailsAsync(
            userCredentials.Username,
            userCredentials.Password
        );
        var thisDeviceSession = await _userSessionService.GetSessionAsync(
            user.Id,
            userCredentials.DeviceName,
            ipAddress
        );
        return thisDeviceSession is null 
            ? await StartUserSessionAsync(user, userCredentials, ipAddress)
            : await RefreshSessionAsync(thisDeviceSession.RefreshToken);
    }

    public async Task<AuthorizationResult> RefreshSessionAsync(string refreshToken)
    {
        var session = await _userSessionService.GetSessionAsync(refreshToken);
        if (session is null)
        {
            throw new BadHttpRequestException("Невозможно обновить сессию, которая была завершена или не существует.");
        }

        var user = await _usersService.GetUserDetailsAsync(session.UserId);
        var (newAccessTokenInfo, newRefreshTokenInfo) = await BuildAccessAndRefreshTokensAsync(user);
        session.RefreshToken = newRefreshTokenInfo.Token;
        session.ExpireDate = newRefreshTokenInfo.ExpireDate;

        return new AuthorizationResult(newAccessTokenInfo, newRefreshTokenInfo, session);
    }

    private async Task<AuthorizationResult> StartUserSessionAsync(
        User user,
        UserCredentialsDto userCredentials,
        string? ipAddress
    )
    {
        var (accessTokenInfo, refreshTokenInfo) = await BuildAccessAndRefreshTokensAsync(user);

        var session = new Session
        {
            RefreshToken = refreshTokenInfo.Token,
            DeviceName = userCredentials.DeviceName,
            IpAddress = ipAddress,
            UserId = user.Id,
            ExpireDate = refreshTokenInfo.ExpireDate,
        };
        await _userSessionService.SaveSessionAsync(session);
        return new AuthorizationResult(accessTokenInfo, refreshTokenInfo, session);
    }

    private async Task<(TokenInfo accessToken, TokenInfo refreshToken)> BuildAccessAndRefreshTokensAsync(User user)
    {
        var userPermissions = await _userRolesService.GetUserRolesAsync(user.Id)
            .ContinueWith(roles => roles.Result.SelectMany(r => r.Permissions));

        var accessToken = _tokenService.BuildAccessToken(user, userPermissions.Select(p => p.Name));
        var refreshToken = _tokenService.BuildRefreshToken();
        return (accessToken, refreshToken);
    }
}