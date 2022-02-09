using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Materials.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class MaterialsController : ControllerBase
{
    private readonly IMaterialsService _materialsService;


    public MaterialsController(IMaterialsService materialsService)
    {
        _materialsService = materialsService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<Material>>> GetMaterialsPageAsync(Page page, BaseFilter filter)
    {
        var materialsPage = await _materialsService.GetMaterialsAsync(page, filter);
        return Ok(materialsPage);
    }

    [HttpPost(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<Material>> AddMaterialAsync(Material material)
    {
        material.Id = Guid.Empty;
        var savedMaterial = await _materialsService.SaveMaterialAsync(material);
        return Ok(savedMaterial);
    }
    
    [HttpPut(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<Material>> SaveMaterialAsync(Material material)
    {
        var savedMaterial = await _materialsService.SaveMaterialAsync(material);
        return Ok(savedMaterial);
    }

    [HttpDelete("{id:guid}")]
    public async Task<NoContentResult> RemoveMaterialAsync(Guid id)
    {
        await _materialsService.RemoveMaterialAsync(id);
        return NoContent();
    }
}