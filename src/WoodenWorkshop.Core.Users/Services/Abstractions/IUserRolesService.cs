using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Users.Services.Abstractions;

public interface IUserRolesService
{
    Task<ICollection<Role>> GetUserRolesAsync(Guid userId);

    Task AssignRoleToUserAsync(Guid userId, Guid roleId);

    Task UnassignRoleFromUserAsync(Guid userId, Guid roleId);
}