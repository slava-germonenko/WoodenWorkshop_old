using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Assets.Dtos;
using WoodenWorkshop.Core.Assets.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Dtos;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly IAssetsService _assetsService;

    private readonly IAssetsBulkActionsService _assetsBulkActionsService;


    public AssetsController(IAssetsService assetsService, IAssetsBulkActionsService assetsBulkActionsService)
    {
        _assetsService = assetsService;
        _assetsBulkActionsService = assetsBulkActionsService;
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Asset>> GetAssetAsync(Guid id)
    {
        var asset = await _assetsService.GetAssetAsync(id);
        return Ok(asset);
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<Asset>>> GetAssetsAsync(
        [FromQuery] Page page,
        [FromQuery] Guid? folderId
    )
    {
        var assets = await _assetsService.GetAssetsAsync(page, folderId);
        return Ok(assets);
    }

    [HttpPost("")]
    public async Task<ActionResult<Asset>> CreateAssetAsync()
    {
        if (!Request.Form.Files.Any())
        {
            throw new BadHttpRequestException("Необходимо прикрепить хотя бы 1 файл к запросу.");
        }
        
        var assetDtos = Request.Form.Files
            .Select(file => new CreateAssetDto(file.Name, file.OpenReadStream()))
            .ToList();

        var assets = await _assetsBulkActionsService.UploadAsync(assetDtos);
        return Ok(assets);
    }

    [HttpPut("")]
    public async Task<ActionResult<Asset>> UpdateAssetDetailsAsync([FromBody] Asset asset)
    {
        var updatedAsset = await _assetsService.UpdateAssetDetailsAsync(asset);
        return Ok(updatedAsset);
    }

    [HttpPatch("")]
    public async Task<NoContentResult> QueueAssetsForRemovalAsync([FromBody] RemoveAssetsDto removeAssetsDto)
    {
        await _assetsBulkActionsService.QueueForRemovalAsync(removeAssetsDto.AssetIds);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<Asset>> ReplaceAssetFileAsync(Guid id)
    {
        if (!Request.Form.Files.Any())
        {
            throw new BadHttpRequestException("Файл не был найден в запросе.");
        }

        var asset = await _assetsService.UpdateAssetBlobAsync(id, Request.Form.Files.First().OpenReadStream());
        return Ok(asset);
    }

    [HttpDelete("{id:guid}")]
    public async Task<NoContentResult> QueueAssetForRemovalAsync(Guid id)
    {
        await _assetsService.MarkAsQueuedForRemovalAsync(id);
        return NoContent();
    }
}