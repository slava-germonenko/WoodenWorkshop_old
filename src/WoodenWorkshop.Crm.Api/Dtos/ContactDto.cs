using System.ComponentModel.DataAnnotations;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Crm.Api.Dtos;

public class ContactDto
{
    public Guid? Id { get; set; }

    public Guid? AssigneeId { get; set; }

    [StringLength(100)]
    public string? FirstName { get; set; }

    [StringLength(100)]
    public string? LastName { get; set; }

    [Required, EmailAddress, StringLength(250)]
    public string EmailAddress { get; set; }

    [Required, Phone, StringLength(20)]
    public string PhoneNumber { get; set; }

    public static implicit operator Contact(ContactDto contactDto) => new()
    {
        Id = contactDto.Id ?? Guid.Empty,
        AssigneeId = contactDto.AssigneeId,
        FirstName = contactDto.FirstName,
        LastName = contactDto.LastName,
        EmailAddress = contactDto.EmailAddress,
        PhoneNumber = contactDto.PhoneNumber,
    };
}