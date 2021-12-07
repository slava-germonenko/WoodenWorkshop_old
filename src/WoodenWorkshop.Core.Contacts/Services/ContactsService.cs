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
        var contactToUpdate = await _context.Contacts.FindAsync(contact.Id);
        if (contactToUpdate is null)
        {
            throw new NotFoundException($"Контакт с идентификатором {contact.Id} не найден.");
        }

        contactToUpdate.CopyFrom(contact);
        _context.Contacts.Update(contactToUpdate);
        await _context.SaveChangesAsync();

        return contactToUpdate;
    }
}