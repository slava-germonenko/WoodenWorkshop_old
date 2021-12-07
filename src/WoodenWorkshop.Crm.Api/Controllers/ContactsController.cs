using Microsoft.AspNetCore.Mvc;

using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Contacts.Models;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Crm.Api.Dtos;

namespace WoodenWorkshop.Crm.Api.Controllers;

[ApiController, Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IContactsListService _contactsListService;

    private readonly IContactsService _contactsService;


    public ContactsController(
        IContactsListService contactsListService,
        IContactsService contactsService
    )
    {
        _contactsListService = contactsListService;
        _contactsService = contactsService;
    }


    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Contact>> GetContactDetails(Guid id)
    {
        var contact = await _contactsService.GetContactDetailsAsync(id);
        return Ok(contact);
    }

    [HttpGet("")]
    public async Task<ActionResult<PagedCollection<Contact>>> GetContactsListAsync(
        [FromQuery] Page page,
        [FromQuery] ContactsFilter contactsFilter
    )
    {
        var contacts = await _contactsListService.GetContactsListAsync(page, contactsFilter);
        return Ok(contacts);
    }

    [HttpPost("")]
    public async Task<ActionResult<Contact>> CreateContactAsync([FromBody] ContactDto contactDto)
    {
        var contact = await _contactsListService.AddContactAsync(contactDto);
        return Ok(contact);
    }

    [HttpPut("")]
    public async Task<ActionResult<Contact>> SaveContactAsync([FromBody] ContactDto contactDto)
    {
        var contact = contactDto.Id is null
            ? await _contactsListService.AddContactAsync(contactDto)
            : await _contactsService.UpdateContactDetailsAsync(contactDto);

        return Ok(contact);
    }

    [HttpDelete("{id:guid}")]
    public async Task<NoContentResult> RemoveContactAsync(Guid id)
    {
        await _contactsListService.RemoveContactAsync(id);
        return NoContent();
    }
}