using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Material : BaseModel
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;
}