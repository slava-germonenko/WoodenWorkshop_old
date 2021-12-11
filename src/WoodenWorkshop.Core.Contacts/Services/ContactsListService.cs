using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Contacts.Models;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Contacts.Services;

public class ContactsListService : IContactsListService
{
    private readonly CoreContext _context;


    public ContactsListService(CoreContext context)
    {
        _context = context;
    }


    public async Task<PagedCollection<Contact>> GetContactsListAsync(Page page, ContactsFilter? contactsFilter = null)
    {
        var contactsQuery = _context.Contacts
            .AsNoTracking();

        if (contactsFilter is not null)
        {
            contactsQuery = contactsQuery
                .WhereNotNull(c => c.AssigneeId == contactsFilter.AssigneeId, contactsFilter.AssigneeId)
                .WhereNotNull(c => c.FirstName == contactsFilter.FirstName, contactsFilter.FirstName)
                .WhereNotNull(c => c.LastName == contactsFilter.LastName, contactsFilter.LastName)
                .WhereNotNull(c => c.EmailAddress == contactsFilter.EmailAddress, contactsFilter.EmailAddress);
        }

        var contacts = await contactsQuery.Include(c => c.Assignee).Page(page).ToListAsync();
        return new(page, contacts);
    }

    public async Task<Contact> AddContactAsync(Contact contact)
    {
        var phoneNumberIsInUse = await _context.Contacts.AnyAsync(c => c.PhoneNumber == contact.PhoneNumber);
        if (phoneNumberIsInUse)
        {
            throw new DuplicateException($"Контакт с номером телефона {contact.PhoneNumber} уже существует.");
        }

        contact.Updated = null;
        contact.Created = DateTime.UtcNow;

        await _context.AddAsync(contact);
        await _context.SaveChangesAsync();

        return contact;
    }

    public async Task RemoveContactAsync(Guid contactId)
    {
        var contact = await _context.Contacts.FindAsync(contactId);
        if (contact is not null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }
}