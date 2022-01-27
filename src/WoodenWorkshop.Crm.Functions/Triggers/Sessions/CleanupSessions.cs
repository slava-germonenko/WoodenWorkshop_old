using Microsoft.Azure.Functions.Worker;

using WoodenWorkshop.Auth.Services.Abstractions;
using WoodenWorkshop.Crm.Functions.Models;

namespace WoodenWorkshop.Crm.Functions.Triggers.Sessions;

public class CleanupSessions
{
    private readonly ISessionsCleanupService _sessionsCleanupService;


    public CleanupSessions(ISessionsCleanupService sessionsCleanupService)
    {
        _sessionsCleanupService = sessionsCleanupService;
    }


    [Function("CleanupSessions")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
    {
        await _sessionsCleanupService.RemoveExpiredSessionsAsync();
    }
}
