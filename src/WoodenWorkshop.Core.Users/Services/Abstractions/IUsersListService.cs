using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Models;

namespace WoodenWorkshop.Core.Users.Services.Abstractions;

public interface IUsersListService
{
    Task<PagedCollection<User>> GetUsersListAsync(Page page, UsersFilter? filter = null);

    Task<User> AddUserAsync(User user);

    Task RemoveUserAsync(Guid id);
}