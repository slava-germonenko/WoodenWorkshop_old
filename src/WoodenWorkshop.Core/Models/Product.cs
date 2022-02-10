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

    public bool IsDeleted { get; set; }

    public ProductItemStatus ItemStatus { get; set; } = ProductItemStatus.None;

    public Guid? MaterialId { get; set; }
    
    public Guid? CategoryId { get; set; }

    public Material? Material { get; set; }
    
    public Category? Category { get; set; }

    public ICollection<ProductAsset> Assets { get; set; } = new List<ProductAsset>();
    
    public ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();
    
    public ICollection<ProductSocialLink> ProductSocialLinks { get; set; } = new List<ProductSocialLink>();

    public void CopyDetails(Product source)
    {
        RussianName = source.RussianName;
        EnglishName = source.EnglishName;
        VendorCode = source.VendorCode;
        Description = source.Description;
        VatRate = source.VatRate;
        Height = source.Height;
        Width = source.Width;
        Depth = source.Depth;
        IsActive = source.IsActive;
        ItemStatus = source.ItemStatus;
        MaterialId = source.MaterialId;
        CategoryId = source.CategoryId;
    }

    public void MarkAsDeleted()
    {
        IsActive = true;
        IsDeleted = true;
    }
}