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
public class AssetsController : ControllerBase
{
    private readonly IAssetsListService _assetsListService;

    private readonly IAssetsService _assetsService;


    public AssetsController(IAssetsListService assetsListService, IAssetsService assetsService)
    {
        _assetsListService = assetsListService;
        _assetsService = assetsService;
    }


    [HttpGet(""), RequirePermissions(Permissions.Assets, Permissions.ViewAssets)]
    public async Task<ActionResult<PagedCollection<Asset>>> GetAssetsListAsync(
        [FromQuery] Page page,
        [FromQuery] AssetsFilter filter
    )
    {
        var assets = await _assetsListService.GetAssetsListAsync(page, filter);
        return Ok(assets);
    }

    [HttpGet("{assetId:guid}"), RequirePermissions(Permissions.Assets, Permissions.ViewAssets)]
    public async Task<ActionResult<Asset>> GetAssetAsync(Guid assetId)
    {
        var asset = await _assetsService.GetAssetAsync(assetId);
        return Ok(asset);
    }

    [HttpPost("")]
    [RequirePermissions(Permissions.Assets, Permissions.UploadAssets)]
    public async Task<ActionResult<PagedCollection<Asset>>> UploadAssetAsync(
        [FromQuery] bool renameDuplicate = false
    )
    {
        var assetFile = Request.Form.Files.FirstOrDefault();
        if (assetFile is null)
        {
            throw new BadHttpRequestException("Файл не найден.");
        }
        var assets = await _assetsListService.AddAssetAsync(
            new AddAssetDto(assetFile.FileName),
            assetFile.OpenReadStream(),
            renameDuplicate
        );
        return Ok(assets);
    }
    
    [HttpPut(""), RequirePermissions(Permissions.Assets, Permissions.UploadAssets)]
    public async Task<ActionResult<PagedCollection<Asset>>> UpdateAssetAsync(
        [FromQuery] bool renameDuplicate,
        [FromBody] Asset asset
    )
    {
        var assets = await _assetsService.UpdateAssetAsync(asset, renameDuplicate);
        return Ok(assets);
    }
    
    [HttpDelete("{assetId:guid}"), RequirePermissions(Permissions.Assets, Permissions.UploadAssets)]
    public async Task<ActionResult<PagedCollection<Asset>>> RemoveAssetAsync(Guid assetId)
    {
        await _assetsListService.RemoveAssetAsync(assetId);
        return NoContent();
    }
}