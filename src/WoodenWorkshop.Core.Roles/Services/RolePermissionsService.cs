using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Roles.Services.Abstractions;

namespace WoodenWorkshop.Core.Roles.Services;

public class RolePermissionsService : IRolePermissionsService
{
    private readonly CoreContext _context;

    private readonly IRolesService _rolesService;


    public RolePermissionsService(CoreContext context, IRolesService rolesService)
    {
        _context = context;
        _rolesService = rolesService;
    }


    public async Task AddPermissionToRoleAsync(Guid roleId, string permission)
    {
        var role = await _rolesService.GetRoleAsync(roleId);
        if (role.Permissions.Any(p => p.Name == permission))
        {
            return;
        }
        
        role.Permissions.Add(new ()
        {
            Name = permission,
        });

        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task RemovePermissionFromRoleAsync(Guid roleId, string permission)
    {
        var role = await _rolesService.GetRoleAsync(roleId);
        if (role.Permissions.All(p => p.Name != permission))
        {
            return;
        }

        role.Permissions.Remove(
            role.Permissions.First(p => p.Name == permission)
        );
        
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task SetRolePermissionsAsync(Guid roleId, ICollection<string> permissionNames)
    {
        var role = await _rolesService.GetRoleAsync(roleId);
        role.Permissions = permissionNames.Select(pn => new Permission
        {
            Name = pn,
        }).ToList();
        
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }
}