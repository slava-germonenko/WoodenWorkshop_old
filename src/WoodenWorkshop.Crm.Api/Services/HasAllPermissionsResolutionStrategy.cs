using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Services;

public class HasAllPermissionsResolutionStrategy : IPermissionsResolutionStrategy
{
    public IReadOnlyCollection<string> RequiredPermissions { get; }
    
    
    public HasAllPermissionsResolutionStrategy(IReadOnlyCollection<string> requiredPermissions)
    {
        RequiredPermissions = requiredPermissions;
    }

    
    public bool PermissionsMeetRequirements(IReadOnlyCollection<string> providedPermissions)
    {
        var matchedPermissions = RequiredPermissions.Intersect(
            providedPermissions,
            StringComparer.InvariantCultureIgnoreCase
        );

        return matchedPermissions.Count() == RequiredPermissions.Count;
    }
}