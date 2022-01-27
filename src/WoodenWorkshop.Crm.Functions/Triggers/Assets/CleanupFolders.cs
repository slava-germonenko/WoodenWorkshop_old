using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Services.Abstractions;

namespace WoodenWorkshop.Crm.Functions.Triggers.Assets;

public class CleanupFolders
{
    private readonly IFoldersCleanupService _foldersCleanupService;


    public CleanupFolders(IFoldersCleanupService foldersCleanupService)
    {
        _foldersCleanupService = foldersCleanupService;
    }


    [Function("CleanupFolders")]
    public async Task Run([ServiceBusTrigger("folders-cleanup")] string messageBody)
    {
        var cleanupFoldersMessage = JsonSerializer.Deserialize<CleanupFoldersMessage>(messageBody);
        if (cleanupFoldersMessage is not null)
        {
            await _foldersCleanupService.CleanUpFoldersAsync(cleanupFoldersMessage.FolderIds);
        }
    }
}