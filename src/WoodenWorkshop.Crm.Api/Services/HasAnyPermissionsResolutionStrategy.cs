using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Services;

public class HasAnyPermissionsResolutionStrategy : IPermissionsResolutionStrategy
{
    public IReadOnlyCollection<string> RequiredPermissions { get; }
    
    
    public HasAnyPermissionsResolutionStrategy(IReadOnlyCollection<string> requiredPermissions)
    {
        RequiredPermissions = requiredPermissions;
    }
    
    
    public bool PermissionsMeetRequirements(IReadOnlyCollection<string> providedPermissions)
    {
        return RequiredPermissions.Any(
            perm => providedPermissions.Contains(perm, StringComparer.InvariantCultureIgnoreCase)
        );
    }
}