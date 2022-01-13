using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Models;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly IFoldersService _foldersService;

    private readonly IFolderListService _folderListService;


    public FoldersController(IFoldersService foldersService, IFolderListService folderListService)
    {
        _foldersService = foldersService;
        _folderListService = folderListService;
    }

    [HttpGet(""), RequirePermissions(Permissions.Assets, Permissions.ViewAssets)]
    public async Task<ActionResult<PagedCollection<AssetFolder>>> GetFoldersListAsync(
        [FromQuery] Page page,
        [FromQuery] FolderFilter folderFilter
    )
    {
        var folders = await _folderListService.GetFoldersListAsync(page, folderFilter);
        return Ok(folders);
    }

    [HttpPost(""), RequirePermissions(Permissions.Assets)]
    public async Task<ActionResult<AssetFolder>> CreateFolderAsync([FromBody] AddFolderDto folderDto)
    {
        var folder = await _folderListService.AddFolderAsync(folderDto);
        return Ok(folder);
    }
    
    [HttpPut(""), RequirePermissions(Permissions.Assets)]
    public async Task<ActionResult<AssetFolder>> UpdateFolderAsync([FromBody] AssetFolder folderDto)
    {
        var folder = await _foldersService.UpdateFolderAsync(folderDto);
        return Ok(folder);
    }
    
    [HttpDelete("{folderId:guid}"), RequirePermissions(Permissions.Assets)]
    public async Task<NoContentResult> RemoveFolderAsync(Guid folderId)
    {
        await _foldersService.RemoveFolderAsync(folderId);
        return NoContent();
    }
}