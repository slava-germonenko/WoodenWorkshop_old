using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Contact : BaseModel
{
    public Guid? AssigneeId { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Required, EmailAddress, StringLength(250)]
    public string EmailAddress { get; set; }

    [Required, Phone, StringLength(30)]
    public string PhoneNumber { get; set; }

    public User? Assignee { get; set; }
}