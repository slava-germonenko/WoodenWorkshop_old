using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WoodenWorkshop.Core.Models;

public class Folder : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public Guid? ParentFolderId { get; set; }

    [JsonIgnore]
    public bool QueuedForRemoval { get; set; }
}