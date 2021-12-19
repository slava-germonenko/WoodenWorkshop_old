using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Users.Services.Abstractions;

public interface IUsersService
{
    Task<User> GetUserDetailsAsync(Guid id);

    Task<User> GetUserDetailsAsync(string emailAddress, string? password = null);

    Task<User> UpdateUserDetailsAsync(User user);
}