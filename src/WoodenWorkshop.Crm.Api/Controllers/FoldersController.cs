using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, RequirePermissions(Permissions.Assets), Route("api/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly IAssetsBulkActionsService _assetsBulkActionsService;
    
    private readonly IFoldersService _foldersService;

    private readonly IFoldersBulkActionsService _foldersBulkActionsService;


    public FoldersController(
        IAssetsBulkActionsService assetsBulkActionsService,
        IFoldersService foldersService,
        IFoldersBulkActionsService foldersBulkActionsService 
    )
    {
        _assetsBulkActionsService = assetsBulkActionsService;
        _foldersService = foldersService;
        _foldersBulkActionsService = foldersBulkActionsService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<Folder>>> GetFoldersAsync(
        [FromQuery] Page page,
        [FromQuery] Guid? parentId
    )
    {
        var folders = await _foldersService.GetFoldersAsync(page, parentId);
        return Ok(folders);
    }

    [HttpPost("")]
    public async Task<ActionResult<Folder>> CreateFolderAsync([FromBody] Folder folder)
    {
        var createdFolder = await _foldersService.CreateAsync(folder);
        return Ok(createdFolder);
    }

    [HttpPut("")]
    public async Task<ActionResult<Folder>> UpdateFolderAsync([FromBody] Folder folder)
    {
        var updatedFolder = await _foldersService.UpdateDetailsAsync(folder);
        return Ok(updatedFolder);
    }

    [HttpPost("{folderId:guid}/assets")]
    public async Task<ActionResult<ICollection<Asset>>> UploadAssetsAsync(Guid folderId)
    {
        if (!Request.Form.Files.Any())
        {
            throw new BadHttpRequestException("Необходимо прикрепить хотя бы 1 файл к запросу.");
        }

        var assetDtos = Request.Form.Files
            .Select(file => new CreateAssetDto(file.Name, file.OpenReadStream(), folderId))
            .ToList();

        var assets = await _assetsBulkActionsService.UploadAsync(assetDtos);
        return Ok(assets);
    }

    [HttpDelete("{folderId:guid}")]
    public async Task<NoContentResult> QueueFolderForRemovalAsync(Guid folderId)
    {
        await _foldersService.QueueForRemovalAsync(folderId);
        return NoContent();
    }

    [HttpPatch("")]
    public async Task<NoContentResult> QueueFoldersForRemovalAsync([FromBody] RemoveFoldersDto foldersDto)
    {
        await _foldersBulkActionsService.QueuedForRemovalAsync(foldersDto.FolderIds);
        return NoContent();
    }
}