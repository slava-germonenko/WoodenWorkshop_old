using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class ProductAsset : BaseModel
{
    [Range(0, int.MaxValue)]
    public int AssetOrder { get; set; }
    
    public Guid ProductId { get; set; }
    
    [Required]
    public Uri Url { get; set; }

    public bool IsExternal { get; set; }

    public void CopyDetails(ProductAsset source)
    {
        Url = source.Url;
        IsExternal = source.IsExternal;
    }
}