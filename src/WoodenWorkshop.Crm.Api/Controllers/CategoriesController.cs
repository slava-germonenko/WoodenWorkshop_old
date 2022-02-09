using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.Products.Categories.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoriesService _categoriesService;


    public CategoriesController(ICategoriesService categoriesService)
    {
        _categoriesService = categoriesService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<Category>>> GetCategoriesPageAsync(
        [FromQuery] Page page,
        [FromQuery] BaseFilter filter
    )
    {
        var categoriesPage = await _categoriesService.GetCategoriesAsync(page, filter);
        return Ok(categoriesPage);
    }
    
    [HttpPost(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<Category>> AddCategoryAsync(Category categoryToSave)
    {
        categoryToSave.Id = Guid.Empty;
        var savedCategoryCopy = await _categoriesService.SaveCategoryAsync(categoryToSave);
        return Ok(savedCategoryCopy);
    }

    [HttpPut(""), RequirePermissions(Permissions.Admin)]
    public async Task<ActionResult<Category>> SaveCategoryAsync(Category categoryToSave)
    {
        var savedCategoryCopy = await _categoriesService.SaveCategoryAsync(categoryToSave);
        return Ok(savedCategoryCopy);
    }

    [HttpDelete("{id:guid}"), RequirePermissions(Permissions.Admin)]
    public async Task<NoContentResult> RemoveCategoryAsync(
        Guid id,
        [FromQuery(Name = "reassignTo")] Guid? reassignToId
    )
    {
        await _categoriesService.RemoveCategoryAsync(id, reassignToId);
        return NoContent();
    }
}