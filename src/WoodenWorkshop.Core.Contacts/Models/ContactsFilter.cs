namespace WoodenWorkshop.Core.Contacts.Models;

public record ContactsFilter
{
    public Guid? AssigneeId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? EmailAddress { get; set; }
}