using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Exceptions;

namespace WoodenWorkshop.Crm.Api.Controllers;

public class UserAwareController : ControllerBase
{
    protected Guid CurrentUserId
    {
        get
        {
            var userId = HttpContext
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
    }

    protected IReadOnlyCollection<string> CurrentUserPermissions
    {
        get
        {
            // perm
            var permissions = HttpContext
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
}