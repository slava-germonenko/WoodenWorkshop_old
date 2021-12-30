using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth.Services.Abstractions;

public interface IUserSessionService
{
    Task ExpireSessionAsync(string refreshToken);

    Task<Session?> GetSessionAsync(string refreshToken);

    Task<IReadOnlyCollection<Session>> GetUserSessionsAsync(Guid userId);

    Task<Session> RefreshSessionAsync(string refreshToken, string newRefreshToken);
    
    Task SaveSessionAsync(Session session);
}