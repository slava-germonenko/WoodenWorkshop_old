using Microsoft.AspNetCore.Mvc.Filters;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Extensions;
using WoodenWorkshop.Crm.Api.Services;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionsAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredPermissions;

    private readonly PermissionResolutionStrategies _permissionResolutionStrategyCode;

    public RequirePermissionsAttribute(PermissionResolutionStrategies resolutionStrategyCode, params string[] permissions)
    {
        _requiredPermissions = permissions;
        _permissionResolutionStrategyCode = resolutionStrategyCode;
    }
    
    public RequirePermissionsAttribute(params string[] permissions)
    {
        _requiredPermissions = permissions;
        _permissionResolutionStrategyCode = PermissionResolutionStrategies.HasAny;
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

        var hasAllRequirePermissions = GetPermissionsResolutionStrategy(_permissionResolutionStrategyCode, _requiredPermissions)
            .PermissionsMeetRequirements(permissions);

        if (!hasAllRequirePermissions)
        {
            throw new UnauthorizedException("У вас недостаточно прав доступа для этой операции.");
        }
    }

    private IPermissionsResolver GetPermissionsResolutionStrategy(
        PermissionResolutionStrategies strategyCode,
        IReadOnlyCollection<string> requiredPermissions
    )
    {
        return strategyCode switch
        {
            PermissionResolutionStrategies.HasAll => new HasAllPermissionsResolver(requiredPermissions),
            PermissionResolutionStrategies.HasAny => new HasAnyPermissionsResolver(requiredPermissions),
            _ => throw new ArgumentOutOfRangeException(nameof(strategyCode), strategyCode, "Невозможно проверить права доступа пользователя.")
        };
    }
}