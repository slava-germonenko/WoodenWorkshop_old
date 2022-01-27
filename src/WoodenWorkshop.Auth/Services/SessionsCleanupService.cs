using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Services.Abstractions;

namespace WoodenWorkshop.Auth.Services;

public class SessionsCleanupService : ISessionsCleanupService
{
    private readonly AuthContext _authContext;


    public SessionsCleanupService(AuthContext authContext)
    {
        _authContext = authContext;
    }


    public async Task RemoveExpiredSessionsAsync()
    {
        var expiredThreshold = DateTime.UtcNow;
        var expireSessions = await _authContext.Sessions
            .AsNoTracking()
            .Where(session => session.ExpireDate <= expiredThreshold)
            .ToListAsync();
        
        _authContext.Sessions.RemoveRange(expireSessions);
        await _authContext.SaveChangesAsync();
    }
}