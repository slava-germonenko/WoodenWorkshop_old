using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Role : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; }
}