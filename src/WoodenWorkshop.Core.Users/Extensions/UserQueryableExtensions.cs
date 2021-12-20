using Microsoft.EntityFrameworkCore;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Users.Extensions;

public static class UserQueryableExtensions
{
    public static IQueryable<User> WithRoles(this IQueryable<User> users)
    {
        return users.Include(u => u.Roles);
    }
}