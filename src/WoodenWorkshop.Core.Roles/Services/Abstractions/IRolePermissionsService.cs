namespace WoodenWorkshop.Core.Roles.Services.Abstractions;

public interface IRolePermissionsService
{
    public Task AddPermissionToRoleAsync(Guid roleId, string permission);

    public Task RemovePermissionFromRoleAsync(Guid roleId, string permission);
    
    public Task SetRolePermissionsAsync(Guid roleId, ICollection<string> permissionNames);
}