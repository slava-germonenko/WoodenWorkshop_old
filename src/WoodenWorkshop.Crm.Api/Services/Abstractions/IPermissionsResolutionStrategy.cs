namespace WoodenWorkshop.Crm.Api.Services.Abstractions;

public interface IPermissionsResolutionStrategy
{
    public IReadOnlyCollection<string> RequiredPermissions { get; }
    
    public bool PermissionsMeetRequirements(IReadOnlyCollection<string> providedPermissions);
}