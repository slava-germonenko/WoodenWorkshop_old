using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Core.Products.Models;
using WoodenWorkshop.Core.Products.Services.Abstractions;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Authorize, Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;

    private readonly IProductAssetsService _productAssetsService;

    private readonly IProductPricesService _productPricesService;


    public ProductsController(
        IProductsService productsService,
        IProductAssetsService productAssetsService,
        IProductPricesService productPricesService
    )
    {
        _productsService = productsService;
        _productAssetsService = productAssetsService;
        _productPricesService = productPricesService;
    }


    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<ProductThumbnail>>> GetProductsPageAsync(
        [FromQuery] Page page,
        [FromQuery] OrderByQuery orderByQuery,
        [FromQuery] ProductsFilter productsFilter
    )
    {
        var productsPage = await _productsService.GetProductsPageAsync(page, productsFilter, orderByQuery);
        return Ok(productsPage);
    }

    [HttpPost(""), RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<ActionResult<Product>> CreateProductAsync(Product product)
    {
        product.Id = Guid.Empty;
        var savedProduct = await _productsService.SaveProductDetailsAsync(product);
        return Ok(savedProduct);
    }
    
    [HttpPut(""), RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<ActionResult<Product>> CreateOrUpdateProductAsync(Product product)
    {
        var savedProduct = await _productsService.SaveProductDetailsAsync(product);
        return Ok(savedProduct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Product>> GetProductAsync(Guid id)
    {
        var product = await _productsService.GetProductAsync(id);
        return Ok(product);
    }

    [HttpDelete("{id:guid}"), RequirePermissions(Permissions.Products, Permissions.RemoveProducts)]
    public async Task<NoContentResult> RemoveProductAsync(Guid id)
    {
        await _productsService.SoftDeleteProductAsync(id);
        return NoContent();
    }

    [HttpPut("{productId:guid}/assets"), RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<ActionResult<ProductAsset>> SaveProductAssetAsync(ProductAsset productAsset, Guid productId)
    {
        productAsset.ProductId = productId;
        var savedProductAsset = await _productAssetsService.SaveProductAssetAsync(productAsset);
        return Ok(savedProductAsset);
    }

    [HttpDelete("{productId:guid}/assets/{assetId:guid}")]
    [RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<NoContentResult> RemoveProductAssetAsync(Guid assetId)
    {
        await _productAssetsService.RemoveProductAssetAsync(assetId);
        return NoContent();
    }

    [HttpPut("{productId:guid}/prices"), RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<ActionResult<PriceType>> CreatePriceTypeAsync(ProductPrice productPrice, Guid productId)
    {
        productPrice.ProductId = productId;
        var savedProductPrice = await _productPricesService.SaveProductPriceAsync(productPrice);
        return Ok(savedProductPrice);
    }

    [HttpDelete("{productId:guid}/prices/{priceTypId:guid}")]
    [RequirePermissions(Permissions.Products, Permissions.ManageProducts)]
    public async Task<NoContentResult> SaveProductPriceAsync(Guid productId, Guid priceTypId)
    {
        await _productPricesService.RemoveProductPriceAsync(productId, priceTypId);
        return NoContent();
    }
}