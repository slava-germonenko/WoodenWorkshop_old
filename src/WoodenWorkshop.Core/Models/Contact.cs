using System.ComponentModel.DataAnnotations;

namespace WoodenWorkshop.Core.Models;

public class Contact : BaseModel
{
    public Guid? AssigneeId { get; set; }

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [Required, EmailAddress, StringLength(250)]
    public string EmailAddress { get; set; }

    [Required, Phone, StringLength(20)]
    public string PhoneNumber { get; set; }

    public User? Assignee { get; set; }

    public void CopyFrom(Contact source)
    {
        AssigneeId = source.AssigneeId;
        FirstName = source.FirstName;
        LastName = source.LastName;
        EmailAddress = source.EmailAddress;
        PhoneNumber = source.PhoneNumber;
    }
}