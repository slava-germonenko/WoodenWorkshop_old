namespace WoodenWorkshop.Core.Roles.Services.Abstractions;

public interface IRolePermissionsService
{
    public Task SetRolePermissionsAsync(Guid roleId, ICollection<string> permissionNames);
}