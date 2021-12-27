using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WoodenWorkshop.Common.Models;
using WoodenWorkshop.Common.Models.Paging;
using WoodenWorkshop.Core.Contacts.Models;
using WoodenWorkshop.Core.Contacts.Services.Abstractions;
using WoodenWorkshop.Core.Models;
using WoodenWorkshop.Core.Models.Enums;
using WoodenWorkshop.Crm.Api.Dtos;
using WoodenWorkshop.Crm.Api.Filters;

namespace WoodenWorkshop.Crm.Api.Controllers;

[Authorize, ApiController, Route("api/[controller]")]
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
    [RequirePermissions(
        PermissionResolutionStrategies.HasAny,
        Permissions.Admin,
        Permissions.Contacts,
        Permissions.ViewContacts
    )]
    public async Task<ActionResult<Contact>> GetContactDetails(Guid id)
    {
        var contact = await _contactsService.GetContactDetailsAsync(id);
        return Ok(contact);
    }

    [HttpGet("")]
    [RequirePermissions(
        PermissionResolutionStrategies.HasAny,
        Permissions.Admin,
        Permissions.Contacts,
        Permissions.ViewContacts
    )]
    public async Task<ActionResult<PagedCollection<ContactDto>>> GetContactsListAsync(
        [FromQuery] Page page,
        [FromQuery] ContactsFilter contactsFilter,
        [FromQuery] OrderByQuery orderByQuery
    )
    {
        var contacts = await _contactsListService.GetContactsListAsync(page, contactsFilter, orderByQuery);
        var contactDtos = new PagedCollection<ContactDto>(
            contacts.Page,
            contacts.Items.Select(c => (ContactDto) c).ToArray(),
            contacts.Total
        );
        return Ok(contactDtos);
    }

    [HttpPost("")]
    [RequirePermissions(
        PermissionResolutionStrategies.HasAny,
        Permissions.Admin,
        Permissions.Contacts,
        Permissions.ManageContacts
    )]
    public async Task<ActionResult<ContactDto>> CreateContactAsync([FromBody] ContactDto contactDto)
    {
        var contact = await _contactsListService.AddContactAsync(contactDto);
        return Ok((ContactDto) contact);
    }

    [HttpPut("")]
    [RequirePermissions(
        PermissionResolutionStrategies.HasAny,
        Permissions.Admin,
        Permissions.Contacts,
        Permissions.ManageContacts
    )]
    public async Task<ActionResult<Contact>> SaveContactAsync([FromBody] ContactDto contactDto)
    {
        var contact = contactDto.Id is null
            ? await _contactsListService.AddContactAsync(contactDto)
            : await _contactsService.UpdateContactDetailsAsync(contactDto);

        return Ok((ContactDto) contact);
    }

    [HttpDelete("{id:guid}")]
    [RequirePermissions(
        PermissionResolutionStrategies.HasAny,
        Permissions.Admin,
        Permissions.Contacts,
        Permissions.RemoveContacts
    )]
    public async Task<NoContentResult> RemoveContactAsync(Guid id)
    {
        await _contactsListService.RemoveContactAsync(id);
        return NoContent();
    }
}