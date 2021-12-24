using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Models;
using WoodenWorkshop.Core.Users.Services.Abstractions;

namespace WoodenWorkshop.Core.Users.Services;

public class UsersListService : IUsersListService
{
    private readonly CoreContext _context;


    public UsersListService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<User>> GetUsersListAsync(
        Page page,
        UsersFilter? filter = null,
        OrderByQuery? orderBy = null
    )
    {
        var usersQuery = _context.Users.AsNoTracking();

        if (orderBy is null)
        {
            usersQuery = usersQuery.OrderByDescending(u => u.Updated);
        }
        else
        {
            usersQuery = orderBy.IsAsc
                ? usersQuery.OrderBy(GetOrderByExpression(orderBy))
                : usersQuery.OrderByDescending(GetOrderByExpression(orderBy));
        }

        if (filter is not null)
        {
            usersQuery = usersQuery
                .WhereNotNull(u => u.EmailAddress.Contains(filter.EmailAddress!), filter.EmailAddress)
                .WhereNotNull(u => u.EmailAddress.Contains(filter.FirstName!), filter.EmailAddress)
                .WhereNotNull(u => u.EmailAddress.Contains(filter.LastName!), filter.EmailAddress);
        }

        if (!string.IsNullOrEmpty(filter?.Search))
        {
            usersQuery = usersQuery
                .Where(u => u.FirstName.Contains(filter.Search)
                            || u.LastName.Contains(filter.Search)
                            || u.EmailAddress.Contains(filter.Search)
                );
        }

        var users = await usersQuery.Page(page).ToListAsync();
        var contacts = await usersQuery.CountAsync();
        return new(page, users, contacts);
    }

    public async Task<User> AddUserAsync(User user)
    {
        var emailAddressInUse = await _context.Users.AnyAsync(u => u.EmailAddress == user.EmailAddress);
        if (emailAddressInUse)
        {
            throw new DuplicateException($"Пользователь с почтой {user.EmailAddress} уже существует.");
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task RemoveUserAsync(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is not null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    private Expression<Func<User, object>> GetOrderByExpression(OrderByQuery orderByQuery)
    {
        return orderByQuery.OrderBy?.ToLower() switch
        {
            "firstname" => user => user.FirstName,
            "lastname" => user => user.LastName,
            "emailaddress" => user => user.EmailAddress,
            _ => user => user.Updated,
        };
    }
}