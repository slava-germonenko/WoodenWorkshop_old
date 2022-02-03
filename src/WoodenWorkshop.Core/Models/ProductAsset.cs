using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class ProductAsset : BaseModel
{
    [Required]
    public Uri Url { get; set; }
    
    public bool IsExternal { get; set; }

    [Range(0, int.MaxValue)]
    public int AssetOrder { get; set; }
}