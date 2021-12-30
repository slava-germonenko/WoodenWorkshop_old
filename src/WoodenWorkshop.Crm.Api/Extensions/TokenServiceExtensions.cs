using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Extensions;

public static class TokenServiceExtensions
{
    public static (TokenInfo access, TokenInfo refresh) BuildAccessAndRefreshTokens(
        this ITokenService tokenService,
        User user
    )
    {
        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .ToArray();
        var accessToken = tokenService.BuildAccessToken(user, permissions);
        var refreshToken = tokenService.BuildRefreshToken();

        return (accessToken, refreshToken);
    }
}