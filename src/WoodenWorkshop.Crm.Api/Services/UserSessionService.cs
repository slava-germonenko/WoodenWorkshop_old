using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Crm.Api.Models;
using WoodenWorkshop.Crm.Api.Options;
using WoodenWorkshop.Crm.Api.Services.Abstractions;

namespace WoodenWorkshop.Crm.Api.Services;

public class UserSessionService : IUserSessionService
{
    private readonly IConnectionMultiplexer _redisConnectionMultiplexer;

    private readonly IOptionsSnapshot<Security> _securityOptions;

    private IDatabase Redis => _redisConnectionMultiplexer.GetDatabase();


    public UserSessionService(
        IConnectionMultiplexer redisConnectionMultiplexer,
        IOptionsSnapshot<Security> securityOptions
    )
    {
        _redisConnectionMultiplexer = redisConnectionMultiplexer;
        _securityOptions = securityOptions;
    }


    public async Task<UserSession> GetUserSession(string refreshToken)
    {
        var sessionRedisValue = await Redis.StringGetAsync(refreshToken);
        if (!sessionRedisValue.HasValue)
        {
            throw new NotFoundException($"Токен {refreshToken} уже не актуален либо не существует.");
        }

        return JsonSerializer.Deserialize<UserSession>(sessionRedisValue)
               ?? throw new JsonException($"Произошла ошибка при попытке сериализовать сессию для токена: {refreshToken}");
    }

    public async Task ExpireUserSession(string refreshToken)
    {
        await Redis.KeyDeleteAsync(
            GetRedisRefreshTokenKey(refreshToken)
        );
    }

    public async Task SaveUserSession(UserSession session)
    {
        var serializedSession = JsonSerializer.Serialize(session);
        await Redis.StringSetAsync(
            GetRedisRefreshTokenKey(session.RefreshToken),
            serializedSession,
            TimeSpan.FromMinutes(_securityOptions.Value.RefreshTokenTtlMinutes)
        );
    }

    private string GetRedisRefreshTokenKey(string token) => $"refresh-token-{token}";
}