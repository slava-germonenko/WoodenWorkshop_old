using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

using WoodenWorkshop.Core.Models.Enums;

namespace WoodenWorkshop.Core.Models;

public class Asset : BaseModel
{
    public AssetType AssetType { get; set; }

    [StringLength(100)]
    public string AssetName { get; set; } = string.Empty;
    
    [JsonIgnore, StringLength(100)]
    public string? BlobName { get; set; }

    public Uri? Url { get; set; }
    
    public Guid? FolderId { get; set; }
    
    public AssetFolder? Folder { get; set; }

    public bool Uploaded => Url is not null;

    public static readonly Dictionary<string, AssetType> FileExtensionToAssetTypeMap = new()
    {
        { "jpeg", AssetType.Image },
        { "jpg", AssetType.Image },
        { "png", AssetType.Image },
        { "pdf", AssetType.Document },
    };

    public static string[] SupportedFileExtensions => FileExtensionToAssetTypeMap.Keys.ToArray();
}