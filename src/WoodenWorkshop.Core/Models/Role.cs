using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Role : BaseModel
{
    public Guid UserId { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; }

    public User User { get; set; }

    public ICollection<Permission> Permissions { get; set; }
}