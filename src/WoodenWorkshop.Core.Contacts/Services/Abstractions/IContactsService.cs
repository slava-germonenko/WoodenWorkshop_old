using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Contacts.Services.Abstractions;

public interface IContactsService
{
    Task<Contact> GetContactDetailsAsync(Guid contactId);

    Task<Contact> UpdateContactDetailsAsync(Contact contact);
}