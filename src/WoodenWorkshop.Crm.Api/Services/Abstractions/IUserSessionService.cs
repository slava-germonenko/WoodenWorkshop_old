using WoodenWorkshop.Crm.Api.Models;

namespace WoodenWorkshop.Crm.Api.Services.Abstractions;

public interface IUserSessionService
{
    Task<UserSession> GetUserSession(string refreshToken);

    Task SaveUserSession(UserSession session);

    Task ExpireUserSession(string refreshToken);
}