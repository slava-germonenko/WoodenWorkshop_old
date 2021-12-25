using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Roles.Models;

namespace WoodenWorkshop.Core.Roles.Services.Abstractions;

public interface IRolesListService
{
    Task<PagedCollection<Role>> GetRolesListAsync(
        Page page,
        RolesFilter? filter = null,
        OrderByQuery? orderByQuery = null
    );

    Task<Role> AddRoleAsync(Role role);

    Task RemoveRoleAsync(Guid roleId);
}