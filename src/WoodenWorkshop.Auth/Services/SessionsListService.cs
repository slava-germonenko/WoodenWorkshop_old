using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;

namespace WoodenWorkshop.Auth.Services;

public class SessionsListService : ISessionsListService
{
    private readonly AuthContext _authContext;


    public SessionsListService(AuthContext authContext)
    {
        _authContext = authContext;
    }


    public async Task<ICollection<Session>> GetSessionsList(Guid userId)
    {
        return await _authContext.Sessions.Where(session => session.UserId == userId).ToListAsync();
    }

    public async Task CreateSessionAsync(Session session)
    {
        await _authContext.Sessions.AddAsync(session);
        await _authContext.SaveChangesAsync();
    }

    public async Task RemoveSession(Guid sessionId)
    {
        var session = await _authContext.Sessions.FindAsync(sessionId);
        if (session is not null)
        {
            _authContext.Sessions.Remove(session);
            await _authContext.SaveChangesAsync();
        }
    }
}