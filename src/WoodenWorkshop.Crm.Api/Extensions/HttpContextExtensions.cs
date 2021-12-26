using System.Text.Json;

using WoodenWorkshop.Common.Exceptions;

namespace WoodenWorkshop.Crm.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetCurrentUserId(this HttpContext context)
    {
        var userId = context
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "uid")
            ?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedException("Пользователь не авторизирован!");
        }

        return Guid.Parse(userId);
    }
    
    public static IReadOnlyCollection<string> GetCurrentUserPermissions(this HttpContext context)
    {
        var permissions = context
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "perm")
            ?.Value;

        if (string.IsNullOrEmpty(permissions))
        {
            throw new UnauthorizedException("Пользователь не авторизирован!");
        }

        return JsonSerializer.Deserialize<IReadOnlyCollection<string>>(permissions) ?? ArraySegment<string>.Empty;
    }
}