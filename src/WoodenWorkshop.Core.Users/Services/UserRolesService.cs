using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models;
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
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new NotFoundException($"Пользователь с индентификатором {userId} не найден.");
        }

        var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
        if (!roleExists)
        {
            throw new NotFoundException($"Роль с идентификатором {roleId} не найдена.");
        }
        
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(
            ur => ur.UserId == userId && ur.RoleId == roleId
        );
        if (userRole is not null)
        {
            return;   
        }
        
        userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
        };
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task UnassignRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(
            ur => ur.UserId == userId && ur.RoleId == roleId
        );
        if (userRole is not null)
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }
}