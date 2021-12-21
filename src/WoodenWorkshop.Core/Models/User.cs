using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class User : BaseModel
{
    [Required, StringLength(100)]
    public string FirstName { get; set; }

    [Required, StringLength(100)]
    public string LastName { get; set; }

    [EmailAddress, Required, StringLength(250)]
    public string EmailAddress { get; set; }

    [Required, StringLength(400)]
    public string PasswordHash { get; set; }

    public ICollection<Role> Roles { get; set; }
    
    public ICollection<Contact> AssignedContacts { get; set; }
}