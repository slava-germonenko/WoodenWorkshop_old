using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Hashing;
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

    public async Task<User> GetUserDetailsAsync(string emailAddress, string? password = null)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с почтой {emailAddress} не найден.");
        }

        if (string.IsNullOrEmpty(password))
        {
            return user;
        }

        using var hashingUtility = new HashingUtility();
        if (hashingUtility.ComputeHash(password) != user.PasswordHash)
        {
            throw new NotFoundException($"Пользователь с почтой {emailAddress} и указанным паролем не найден.");
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

        var emailInUseByAnotherUser = await _context.Users
            .AnyAsync(u => u.EmailAddress == user.EmailAddress && u.Id != user.Id);

        if (emailInUseByAnotherUser)
        {
            throw new DuplicateException($"Пользователь с почтой {user.EmailAddress} уже существует.");
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }
}