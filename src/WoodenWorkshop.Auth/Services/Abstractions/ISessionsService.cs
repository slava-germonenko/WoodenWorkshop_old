using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth.Services.Abstractions;

public interface ISessionsService
{
    Task<IReadOnlyCollection<Session>> GetSessionsToExpire(int limit);

    Task ExpireSessions(IEnumerable<Session> refreshTokens);
}