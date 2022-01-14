using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class AssetFolder : BaseModel
{
    [StringLength(100)]
    public string Name { get; set; }

    public Guid? ParentFolderId { get; set; }
}