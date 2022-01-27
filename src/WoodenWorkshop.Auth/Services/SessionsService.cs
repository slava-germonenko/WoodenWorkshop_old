using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Auth.Services.Abstractions;

namespace WoodenWorkshop.Auth.Services;

public class SessionsService : ISessionsService
{
    private readonly AuthContext _context;


    public SessionsService(AuthContext context)
    {
        _context = context;
    }


    public async Task<Session?> GetUserSessionByDeviceAndIp(Guid userId, string deviceName, string ipAddress)
    {
        return await _context.Sessions.FirstOrDefaultAsync(
            session => session.UserId == userId && session.DeviceName == deviceName && session.IpAddress == ipAddress
        );
    }

    public async Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Sessions.FirstOrDefaultAsync(session => session.RefreshToken == refreshToken);
    }

    public async Task<Session> UpdateSessionAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }
}