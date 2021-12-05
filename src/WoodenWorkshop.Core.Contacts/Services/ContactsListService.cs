using Microsoft.EntityFrameworkCore;

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


    public async Task<IReadOnlyCollection<Contact>> GetContactsListAsync(ContactsFilter contactsFilter, Page page)
    {
        return await _context.Contacts
            .Include(c => c.Assignee)
            .WhereNotNull(c => c.AssigneeId == contactsFilter.AssigneeId, contactsFilter.AssigneeId)
            .WhereNotNull(c => c.FirstName == contactsFilter.FirstName, contactsFilter.FirstName)
            .WhereNotNull(c => c.LastName == contactsFilter.LastName, contactsFilter.LastName)
            .WhereNotNull(c => c.EmailAddress == contactsFilter.EmailAddress, contactsFilter.EmailAddress)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Contact> AddContactAsync(Contact contact)
    {
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