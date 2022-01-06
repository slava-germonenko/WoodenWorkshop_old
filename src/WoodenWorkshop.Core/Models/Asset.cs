using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class Asset : BaseModel
{
    public AssetType AssetType { get; set; }

    public string Name { get; set; }

    public Uri? Url { get; set; }

    public bool Uploaded => Url is not null;
}