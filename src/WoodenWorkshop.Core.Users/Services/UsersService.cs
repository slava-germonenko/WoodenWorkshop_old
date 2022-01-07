using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Hashing;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Users.Extensions;
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
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user is null)
        {
            throw new NotFoundException($"Пользователь с индентификатором {id} не найден.");
        }

        return user;
    }

    public async Task<User> GetUserDetailsAsync(string emailAddress, string? password = null)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailAddress == emailAddress);
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
        var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (userToUpdate is null)
        {
            throw new NotFoundException($"Пользователь с индентификатором {user.Id} не найден.");
        }

        var emailInUseByAnotherUser = await _context.Users
            .AnyAsync(u => u.EmailAddress == user.EmailAddress && u.Id != user.Id);

        if (emailInUseByAnotherUser)
        {
            throw new DuplicateException($"Пользователь с почтой {user.EmailAddress} уже существует.");
        }

        userToUpdate.CopyDetails(user);
        _context.Users.Update(userToUpdate);
        await _context.SaveChangesAsync();
        return user;
    }
}