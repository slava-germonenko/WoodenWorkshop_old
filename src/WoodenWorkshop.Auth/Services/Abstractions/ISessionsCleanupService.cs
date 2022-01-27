namespace WoodenWorkshop.Auth.Services.Abstractions;

public interface ISessionsCleanupService
{
    public Task RemoveExpiredSessionsAsync();
}