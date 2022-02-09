using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.PriceTypes.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/price-types")]
public class PriceTypesController : ControllerBase
{
    private readonly IPriceTypesService _priceTypesService;


    public PriceTypesController(IPriceTypesService priceTypesService)
    {
        _priceTypesService = priceTypesService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<PriceType>>> GetPriceTypesPageAsync(
        Page page,
        BaseFilter filter
    )
    {
        var priceTypesPage = await _priceTypesService.GetPriceTypesAsync(page, filter);
        return Ok(priceTypesPage);
    }
    
    [HttpPost(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<PriceType>> AddPriceTypeAsync([FromBody] PriceType sourcePriceType)
    {
        sourcePriceType.Id = Guid.Empty;
        var savedPriceType = await _priceTypesService.SavePriceTypeAsync(sourcePriceType);
        return Ok(savedPriceType);
    }

    [HttpPut(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<PriceType>> SavePriceTypeAsync([FromBody] PriceType sourcePriceType)
    {
        var savedPriceType = await _priceTypesService.SavePriceTypeAsync(sourcePriceType);
        return Ok(savedPriceType);
    }

    [HttpDelete("{id:guid}")]
    public async Task<NoContentResult> RemovePriceTypeAsync(Guid id)
    {
        await _priceTypesService.RemovePriceTypeAsync(id);
        return NoContent();
    }
}