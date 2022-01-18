using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WoodenWorkshop.Core.Models;

public class Asset : BaseModel
{
    [StringLength(100)]
    public string AssetName { get; set; } = string.Empty;

    public long Size { get; set; }
    
    [JsonIgnore, StringLength(100)]
    public string? BlobName { get; set; }

    public Uri? Url { get; set; }
    
    public Guid? FolderId { get; set; }
    
    public Folder? Folder { get; set; }

    [JsonIgnore]
    public bool QueuedForRemoval { get; set; }

    public bool Uploaded => Url is not null;

    public static readonly string[] SupportedFileExtensions =
    {
        "doc",
        "docx",
        "jpg",
        "jpeg",
        "pdf",
        "png",
        "svg",
        "txt",
        "xls",
        "xlsx",
    };
}