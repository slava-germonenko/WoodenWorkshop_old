using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Roles.Services.Abstractions;

namespace WoodenWorkshop.Core.Roles.Services;

public class RolesService : IRolesService
{
    private readonly CoreContext _context;


    public RolesService(CoreContext context)
    {
        _context = context;
    }


    public async Task<Role> GetRoleAsync(Guid id)
    {
        var role = await _context.Roles
            .AsNoTracking()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role is null)
        {
            throw new NotFoundException($"Роль с идентификатором {id} не найдена.");
        }

        return role;
    }

    public async Task<Role> UpdateRoleAsync(Role role)
    {
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == role.Id);
        if (!roleExists)
        {
            throw new NotFoundException($"Роль с идентификатором {role.Id} не найдена.");
        }

        _context.Roles.Update(role);
        await _context.SaveChangesAsync();

        return role;
    }
}