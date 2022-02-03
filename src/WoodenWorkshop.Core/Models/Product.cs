using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Product : BaseModel
{
    [StringLength(100)]
    public string RussianName { get; set; } = string.Empty;

    [StringLength(100)]
    public string EnglishName { get; set; } = string.Empty;

    [StringLength(100)]
    public string VendorCode { get; set; } = string.Empty;
    
    public Category Category { get; set; }

    public ICollection<ProductAsset> ProductAssets { get; set; }

    public ICollection<ProductPrice> ProductPrices { get; set; }
}