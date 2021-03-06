using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Options;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Services;

public class JwtService : ITokenService
{
    private const string UserIdClaimName = "uid";

    private const string PermissionsClaimName = "perm";

    private readonly IOptionsSnapshot<Security> _securityOptions;

    private string JwtSecret => _securityOptions.Value.JwtSecret;

    private int AccessTokenTtl => _securityOptions.Value.AccessTokenTtlSeconds;

    private SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(JwtSecret));


    public JwtService(IOptionsSnapshot<Security> securityOptions)
    {
        _securityOptions = securityOptions;
    }


    public TokenInfo BuildAccessToken(User user, IEnumerable<string>? permissions = null)
    {
        var issueDate = DateTime.UtcNow;
        var expireDate = issueDate.AddSeconds(AccessTokenTtl);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = CreateClaimsIdentity(user, permissions),
            Expires = expireDate,
            SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new(
            tokenHandler.WriteToken(token),
            issueDate,
            expireDate
        );
    }

    public TokenInfo BuildRefreshToken()
    {
        var random = RandomNumberGenerator.GetBytes(64);
        var issueDate = DateTime.Now;
        return new TokenInfo(
            Convert.ToBase64String(random),
            issueDate,
            issueDate.AddMinutes(_securityOptions.Value.RefreshTokenTtlMinutes)
        );
    }

    public bool TryGetClaims(string token, out ClaimsPrincipal? claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            claims = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                IssuerSigningKey = SecurityKey,
                ClockSkew = TimeSpan.Zero,
            }, out _);

            return true;
        }
        catch (Exception)
        {
            claims = null;
            return false;
        }
    }

    private ClaimsIdentity CreateClaimsIdentity(User user, IEnumerable<string>? permissions = null)
    {
        var claims = new List<Claim>(2)
        {
            new(UserIdClaimName, user.Id.ToString())
        };

        if (permissions is not null)
        {
            var serializedPermissions = JsonSerializer.Serialize(permissions);
            claims.Add(new(PermissionsClaimName, serializedPermissions));
        }

        return new(claims);
    }
}