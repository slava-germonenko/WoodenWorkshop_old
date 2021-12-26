using Microsoft.AspNetCore.Mvc.Filters;
using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Crm.Api.Extensions;

namespace WoodenWorkshop.Crm.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionsAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredPermissions;
    
    public RequirePermissionsAttribute(params string[] permissions)
    {
        _requiredPermissions = permissions;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!_requiredPermissions.Any())
        {
            return;
        }
        
        var permissions = context.HttpContext.GetCurrentUserPermissions();
        var hasAllRequirePermissions = permissions
            .Intersect(_requiredPermissions)
            .ToList()
            .Count >= _requiredPermissions.Length;

        if (!hasAllRequirePermissions)
        {
            throw new UnauthorizedException("У вас недостаточно прав доступа для этой операции.");
        }
        
    }
}