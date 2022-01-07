using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;

namespace WoodenWorkshop.Auth.Services;

public class SessionsExpirationService : ISessionsExpirationService
{
    private readonly AuthContext _context;


    public SessionsExpirationService(AuthContext context)
    {
        _context = context;
    }


    public async Task<IReadOnlyCollection<Session>> GetSessionsToExpire(int limit)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.ExpireDate < DateTime.UtcNow)
            .OrderBy(s => s.ExpireDate)
            .Take(limit)
            .ToListAsync();
    }

    public async Task ExpireSessions(IEnumerable<Session> refreshTokens)
    {
        _context.Sessions.RemoveRange(refreshTokens);
        await _context.SaveChangesAsync();
    }
}