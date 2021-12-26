using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Roles.Services.Abstractions;

public interface IRolesService
{
    Task<Role> GetRoleAsync(Guid id);

    Task<Role> UpdateRoleAsync(Role role);
}