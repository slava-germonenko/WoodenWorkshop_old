using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Contacts.Models;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Contacts.Services.Abstractions;

public interface IContactsListService
{
    Task<IReadOnlyCollection<Contact>> GetContactsListAsync(ContactsFilter contactsFilter, Page page);

    Task<Contact> AddContactAsync(Contact contact);

    Task RemoveContactAsync(Guid contactId);
}