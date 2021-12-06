using Microsoft.EntityFrameworkCore;

using WoodenWorkshop.Common.Exceptions;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Models;

namespace WoodenWorkshop.Core.Contacts.Services;

public class ContactsService : IContactsService
{
    private readonly CoreContext _context;


    public ContactsService(CoreContext context)
    {
        _context = context;
    }


    public async Task<Contact> GetContactDetailsAsync(Guid contactId)
    {
        var contact = await _context.Contacts.FindAsync(contactId);
        if (contact is null)
        {
            throw new NotFoundException($"Контакт с идентификатором {contactId} не найден.");
        }

        return contact;
    }

    public async Task<Contact> UpdateContactDetailsAsync(Contact contact)
    {
        var contactExists = await _context.Contacts.AnyAsync(c => c.Id == contact.Id);
        if (!contactExists)
        {
            throw new NotFoundException($"Контакт с идентификатором {contact.Id} не найден.");
        }

        contact.Updated = DateTime.UtcNow;
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
        return contact;
    }
}