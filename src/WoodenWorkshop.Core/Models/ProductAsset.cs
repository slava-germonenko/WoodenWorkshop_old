using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class ProductAsset : BaseModel
{
    public AssetType AssetType { get; set; }

    public bool External { get; set; }
    
    public int Order { get; set; }

    public Guid ProductId { get; set; }

    public Uri Url { get; set; }
}