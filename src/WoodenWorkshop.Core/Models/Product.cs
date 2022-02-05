using System.ComponentModel.DataAnnotations;

using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class Product : BaseModel
{
    [Required, StringLength(100)]
    public string RussianName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string EnglishName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string VendorCode { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue)]
    public int Height { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Width { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Depth { get; set; }

    [Range(0, 1)]
    public decimal VatRate { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public ProductItemStatus ItemStatus { get; set; } = ProductItemStatus.None;

    public Guid? MaterialId { get; set; }

    public Material? Material { get; set; }

    public ICollection<ProductAssets> Assets { get; set; } = new List<ProductAssets>();

    public ICollection<ProductSocialLink> ProductSocialLinks { get; set; } = new List<ProductSocialLink>();

    public ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
}