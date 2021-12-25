using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Roles.Models;
using WoodenWorkshop.Core.Roles.Services.Abstractions;

namespace WoodenWorkshop.Core.Roles.Services;

public class RolesListService : IRolesListService
{
    private readonly CoreContext _context;


    public RolesListService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Role>> GetRolesListAsync(
        Page page,
        RolesFilter? filter = null,
        OrderByQuery? orderByQuery = null
    )
    {
        var rolesQuery = _context.Roles.AsNoTracking();
        if (orderByQuery is not null)
        {
            rolesQuery = orderByQuery.IsAsc
                ? rolesQuery.OrderBy(CreateOrderByExpression(orderByQuery))
                : rolesQuery.OrderByDescending(CreateOrderByExpression(orderByQuery));
        }
        else
        {
            rolesQuery = rolesQuery.OrderByDescending(r => r.Created);
        }

        if (filter is not null)
        {
            rolesQuery = rolesQuery.WhereNotNull(r => r.Name.Contains(filter.Name!), filter.Name);
        }

        var roles = await rolesQuery
            .Page(page)
            .Include(r => r.Users)
            .ToListAsync();
        var rolesCount = await rolesQuery.CountAsync();
        return new(page, roles, rolesCount);
    }

    public async Task<Role> AddRoleAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task RemoveRoleAsync(Guid roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role is not null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

    private Expression<Func<Role, object>> CreateOrderByExpression(OrderByQuery orderByQuery)
    {
        return orderByQuery.OrderBy?.ToLower() switch
        {
            "name" => role => role.Name,
            "assigneescount" => role => role.Users.Count,
            _ => role => role.Created,
        };
    }
}