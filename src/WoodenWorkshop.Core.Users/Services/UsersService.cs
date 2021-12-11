using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Services.Abstractions;

namespace WoodenWorkshop.Core.Users.Services;

public class UsersService : IUsersService
{
    private readonly CoreContext _context;


    public UsersService(CoreContext context)
    {
        _context = context;
    }


    public async Task<User> GetUserDetailsAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с индентификатором {id} не найден.");
        }

        return user;
    }

    public async Task<User> GetUserDetailsAsync(string emailAddress)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с почтой {emailAddress} не найден.");
        }

        return user;
    }

    public async Task<User> UpdateUserDetailsAsync(User user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == user.Id);
        if (!userExists)
        {
            throw new NotFoundException($"Пользователь с индентификатором {user.Id} не найден.");
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}