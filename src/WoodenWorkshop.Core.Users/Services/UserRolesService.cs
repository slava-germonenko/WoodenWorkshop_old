using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Extensions;
using WoodenWorkshop.Core.Users.Services.Abstractions;

namespace WoodenWorkshop.Core.Users.Services;

public class UserRolesService : IUserRolesService
{
    private readonly CoreContext _context;


    public UserRolesService(CoreContext context)
    {
        _context = context;
    }


    public async Task<ICollection<Role>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Include(ur => ur.Role)
            .ThenInclude(ur => ur.Permissions)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .ToListAsync();
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var user = await _context.Users.AsNoTracking()
            .WithRoles()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new NotFoundException($"Пользователь с идентификатором {userId} не найден.");
        }

        if (user.Roles.Any(r => r.Id == roleId))
        {
            return;
        }

        var role = await _context.Roles.FindAsync(roleId);
        if (role is null)
        {
            throw new NotFoundException($"Роль с идентификатором {userId} не найдена.");
        }
        
        user.Roles.Add(role);
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task UnassignRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var user = await _context.Users.AsNoTracking()
            .WithRoles()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new NotFoundException($"Пользователь с идентификатором {userId} не найден.");
        }

        var role = user.Roles.FirstOrDefault(r => r.Id == roleId);
        if (role is not null)
        {
            user.Roles.Remove(role);
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}