using System.Security.Claims;

using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Models;

namespace WoodenWorkshop.Crm.Api.Services.Abstractions;

public interface ITokenService
{
    TokenInfo BuildAccessToken(User user, IEnumerable<string>? permissions = null);

    TokenInfo BuildRefreshToken();

    bool TryGetClaims(string token, out ClaimsPrincipal? claims);
}