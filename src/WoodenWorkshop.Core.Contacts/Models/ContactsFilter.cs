namespace WoodenWorkshop.Core.Contacts.Models;

public record ContactsFilter(
    Guid? AssigneeId,
    string? FirstName,
    string? LastName,
    string? EmailAddress,
    string? PhoneNumber,
    string? Search
);
