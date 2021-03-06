using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Common.Extensions;
using WoodenWorkshop.Common.Models;
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


    public async Task<PagedCollection<Contact>> GetContactsListAsync(
        Page page,
        ContactsFilter? filter = null,
        OrderByQuery? orderByQuery = null
    )
    {
        var contactsQuery = _context.Contacts
            .AsNoTracking();

        if (orderByQuery is not null)
        {
            contactsQuery = orderByQuery.IsAsc
                ? contactsQuery.OrderBy(GetContactOrderByExpression(orderByQuery))
                : contactsQuery.OrderByDescending(GetContactOrderByExpression(orderByQuery));
        }
        else
        {
            contactsQuery = contactsQuery.OrderByDescending(contact => contact.Created);
        }

        if (filter is not null)
        {
            contactsQuery = contactsQuery
                .WhereNotNull(c => c.AssigneeId == filter.AssigneeId, filter.AssigneeId)
                .WhereNotNull(c => c.FirstName!.Contains(filter.FirstName!), filter.FirstName)
                .WhereNotNull(c => c.LastName!.Contains(filter.LastName!), filter.LastName)
                .WhereNotNull(c => c.EmailAddress!.Contains(filter.EmailAddress!), filter.EmailAddress)
                .WhereNotNull(c => c.PhoneNumber!.Contains(filter.PhoneNumber!), filter.PhoneNumber);;
        }
        
        if (!string.IsNullOrEmpty(filter?.Search))
        {
            contactsQuery = contactsQuery
                .Where(u => u.FirstName.Contains(filter.Search)
                            || u.LastName.Contains(filter.Search)
                            || u.EmailAddress.Contains(filter.Search)
                            || u.PhoneNumber.Contains(filter.Search)
                );
        }

        var contacts = await contactsQuery.Include(c => c.Assignee).Page(page).ToListAsync();
        var contactsCount = await contactsQuery.CountAsync();
        return new(page, contacts, contactsCount);
    }

    public async Task<Contact> AddContactAsync(Contact contact)
    {
        var phoneNumberIsInUse = await _context.Contacts.AnyAsync(c => c.PhoneNumber == contact.PhoneNumber);
        if (phoneNumberIsInUse)
        {
            throw new DuplicateException($"?????????????? ?? ?????????????? ???????????????? {contact.PhoneNumber} ?????? ????????????????????.");
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

    private Expression<Func<Contact, object>> GetContactOrderByExpression(OrderByQuery orderByQuery)
    {
        return orderByQuery.OrderBy?.ToLower() switch
        {
            "firstname" => contact => contact.FirstName,
            "lastname" => contact => contact.LastName,
            "emailaddress" => contact => contact.EmailAddress,
            "phonenumber" => contact => contact.PhoneNumber,
            _ => contact => contact.Created
        };
    }
}