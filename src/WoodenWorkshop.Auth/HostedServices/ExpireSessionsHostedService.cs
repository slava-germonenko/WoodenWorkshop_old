using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Auth.HostedServices.Settings;
using WoodenWorkshop.Auth.Models;
using WoodenWorkshop.Infrastructure.HostedServices;

namespace WoodenWorkshop.Auth.HostedServices;

public class ExpireSessionsHostedService : IScopedHostedService
{
    private readonly AuthContext _authContext;

    private readonly IExpireTokenServiceSettings _settings;


    public ExpireSessionsHostedService(AuthContext authContext, IExpireTokenServiceSettings settings)
    {
        _authContext = authContext;
        _settings = settings;
    }


    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var sessions = await GetSessionsToExpire();
        await RemoveSessions(sessions);
    }

    private async Task<ICollection<Session>> GetSessionsToExpire()
    {
        return await _authContext.Sessions.AsNoTracking()
            .Where(session => session.ExpireDate < DateTime.UtcNow)
            .OrderBy(session => session.ExpireDate)
            .Take(_settings.ExpireRefreshTokenProcessLimit)
            .ToListAsync();
    }

    private async Task RemoveSessions(IEnumerable<Session> sessions)
    {
        _authContext.Sessions.RemoveRange(sessions);
        await _authContext.SaveChangesAsync();
    }
}