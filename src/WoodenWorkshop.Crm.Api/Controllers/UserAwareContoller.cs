using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Crm.Api.Extensions;

namespace WoodenWorkshop.Crm.Api.Controllers;

public class UserAwareController : ControllerBase
{
    protected Guid CurrentUserId => HttpContext.GetCurrentUserId();

    protected IReadOnlyCollection<string> CurrentUserPermissions => HttpContext.GetCurrentUserPermissions();
}