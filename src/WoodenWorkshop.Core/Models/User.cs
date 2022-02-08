using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class User : BaseModel
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;
    

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress, Required, StringLength(250)]
    public string EmailAddress { get; set; } = string.Empty;

    [Required, StringLength(400)]
    public string PasswordHash { get; set; } = string.Empty;
}