using Microsoft.AspNetCore.Mvc.Filters;
using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Extensions;

namespace WoodenWorkshop.Crm.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionsAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredPermissions;

    private readonly PermissionResolutionStrategies _permissionResolutionStrategies;

    public RequirePermissionsAttribute(PermissionResolutionStrategies resolutionStrategy, params string[] permissions)
    {
        _requiredPermissions = permissions;
        _permissionResolutionStrategies = resolutionStrategy;
    }
    
    public RequirePermissionsAttribute(params string[] permissions)
    {
        _requiredPermissions = permissions;
        _permissionResolutionStrategies = PermissionResolutionStrategies.HasAny;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!_requiredPermissions.Any())
        {
            return;
        }
        
        var permissions = context.HttpContext.GetCurrentUserPermissions();
        if (permissions.Any(p => p == Permissions.Admin))
        {
            return;
        }

        var hasAllRequirePermissions = _permissionResolutionStrategies switch
        {
            PermissionResolutionStrategies.HasAll => HasAllPermissions(permissions),
            PermissionResolutionStrategies.HasAny => HasAnyPermission(permissions),
            _ => false
        };

        if (!hasAllRequirePermissions)
        {
            throw new UnauthorizedException("У вас недостаточно прав доступа для этой операции.");
        }
    }

    private bool HasAllPermissions(IReadOnlyCollection<string> requiredPermissions) => requiredPermissions
        .Intersect(_requiredPermissions)
        .ToList()
        .Count >= _requiredPermissions.Length;
    
    private bool HasAnyPermission(IReadOnlyCollection<string> requiredPermissions) => requiredPermissions
        .Intersect(_requiredPermissions)
        .ToList()
        .Any();
}