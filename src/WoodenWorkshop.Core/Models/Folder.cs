using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WoodenWorkshop.Core.Models;

public class Folder : BaseModel
{
    [StringLength(100)]
    public string Name { get; set; }

    public Guid? ParentFolderId { get; set; }

    [JsonIgnore]
    public bool QueuedForRemoval { get; set; }
}