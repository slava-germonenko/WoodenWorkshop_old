using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Role : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; }

    public ICollection<User> Users { get; set; }

    public ICollection<Permission> Permissions { get; set; }
}