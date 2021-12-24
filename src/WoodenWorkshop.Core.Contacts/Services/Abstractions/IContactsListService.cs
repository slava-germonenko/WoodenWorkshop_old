using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Contacts.Models;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Contacts.Services.Abstractions;

public interface IContactsListService
{
    Task<PagedCollection<Contact>> GetContactsListAsync(
        Page page,
        ContactsFilter? filter = null,
        OrderByQuery? orderByQuery = null
    );

    Task<Contact> AddContactAsync(Contact contact);

    Task RemoveContactAsync(Guid contactId);
}