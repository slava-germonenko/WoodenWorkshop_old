using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Products.Models;

public record ProductThumbnail
{
    public Guid Id { get; set; }
    
    public string RussianName { get; set; } = string.Empty;

    public string EnglishName { get; set; } = string.Empty;

    public string VendorCode { get; set; } = string.Empty;

    public ProductSize Size { get; set; } = new();
    
    public bool IsActive { get; set; }
    
    public ProductItemStatus Status { get; set; }
    
    public Category? Category { get; set; }
    
    public Material? Material { get; set; }
}