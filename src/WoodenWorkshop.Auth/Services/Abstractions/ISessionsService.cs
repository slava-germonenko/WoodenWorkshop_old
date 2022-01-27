using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth.Services.Abstractions;

public interface ISessionsService
{
    Task<Session?> GetUserSessionByDeviceAndIp(Guid userId, string deviceName, string ipAddress);
    
    Task<Session?> GetSessionByRefreshTokenAsync(string refreshToken);

    Task<Session> UpdateSessionAsync(Session session);
}