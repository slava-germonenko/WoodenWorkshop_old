using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class ProductAssets : BaseModel
{
    [Range(0, int.MaxValue)]
    public int AssetOrder { get; set; }
    
    public Guid ProductId { get; set; }
    
    [Required]
    public Uri Url { get; set; }

    public bool IsExternal { get; set; }
}