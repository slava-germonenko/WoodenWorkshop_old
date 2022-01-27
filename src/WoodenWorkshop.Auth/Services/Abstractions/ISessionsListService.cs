using WoodenWorkshop.Auth.Models;

namespace WoodenWorkshop.Auth.Services.Abstractions;

public interface ISessionsListService
{
    public Task<ICollection<Session>> GetSessionsList(Guid userId);

    public Task CreateSessionAsync(Session session);

    public Task RemoveSession(Guid sessionId);
}