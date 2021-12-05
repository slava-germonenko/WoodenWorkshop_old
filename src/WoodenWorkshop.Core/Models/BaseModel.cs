using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public abstract class BaseModel
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}