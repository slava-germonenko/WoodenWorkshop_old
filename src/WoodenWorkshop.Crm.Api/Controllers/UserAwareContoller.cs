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
}