using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Common.Exceptions;

namespace WoodenWorkshop.Auth.Services;

public class UserSessionService : IUserSessionService
{
    private readonly AuthContext _authContext;


    public UserSessionService(AuthContext authContext)
    {
        _authContext = authContext;
    }


    public async Task ExpireSessionAsync(string refreshToken)
    {
        var session = await GetSessionAsync(refreshToken);
        if (session is not null)
        {
            _authContext.Sessions.Remove(session);
            await _authContext.SaveChangesAsync();
        }
    }

    public async Task ExpireSessionAsync(Guid id)
    {
        var session = await _authContext.Sessions.FindAsync(id);
        if (session is not null)
        {
            _authContext.Sessions.Remove(session);
            await _authContext.SaveChangesAsync();
        }
    }

    public async Task<Session?> GetSessionAsync(string refreshToken)
    {
        return await _authContext.Sessions.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken);
    }

    public async Task<Session> GetSessionAsync(Guid id)
    {
        var session = await _authContext.Sessions.FindAsync(id);
        if (session is null)
        {
            throw new NotFoundException("");
        }

        return session;
    }

    public async Task<Session?> GetSessionAsync(Guid userId, string? deviceName, string? ipAddress)
    {
        return await _authContext.Sessions.FirstOrDefaultAsync(
            session => session.UserId == userId && session.DeviceName == deviceName && session.IpAddress == ipAddress
        );
    }

    public async Task<IReadOnlyCollection<Session>> GetUserSessionsAsync(Guid userId)
    {
        return await _authContext.Sessions
            .AsTracking()
            .Where(s => s.UserId == userId)
            .ToArrayAsync();
    }

    public async Task<Session> RefreshSessionAsync(string refreshToken, string newRefreshToken)
    {
        var session = await GetSessionAsync(refreshToken);
        if (session is null)
        {
            throw new UnauthorizedException("Невозможно обновить сессию, которая была завершена.");
        }
        await ExpireSessionAsync(session.RefreshToken);
        session.RefreshToken = newRefreshToken;
        await SaveSessionAsync(session);
        return session;
    }

    public async Task SaveSessionAsync(Session session)
    {
        _authContext.Sessions.Update(session);
        await _authContext.SaveChangesAsync();
    }
}