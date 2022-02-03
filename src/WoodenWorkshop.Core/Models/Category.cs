using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Category : BaseModel
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true;
}