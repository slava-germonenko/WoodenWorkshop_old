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

    public async Task SetRolePermissionsAsync(Guid roleId, ICollection<string> permissionNames)
    {
        var role = await _rolesService.GetRoleAsync(roleId);
        _context.Permissions.RemoveRange(role.Permissions);

        var newPermissions = permissionNames
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(p => new Permission
            {
                Name = p.ToLower(),
                RoleId = roleId,
            });
        await _context.Permissions.AddRangeAsync(newPermissions);
        await _context.SaveChangesAsync();
    }
}