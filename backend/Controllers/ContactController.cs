using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.Contact.Requests;
using backend.ViewModels.Contact.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for contact form submissions.</summary>
[ApiController]
[Route("api/contacts")]
public class ContactController : ControllerBase
{
    private readonly ILogger<ContactController> _logger;
    private readonly ContactServices _contactServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public ContactController(ILogger<ContactController> logger, ContactServices contactServices)
    {
        _logger = logger;
        _contactServices = contactServices;
    }

    /// <summary>Returns a paginated list of contact submissions.</summary>
    [HttpGet]
    public async Task<ActionResult<PaginationItem<ContactResponse>>> GetContacts([FromQuery] PaginationRequest request)
    {
        var result = await _contactServices.GetPaginationAsync(request);
        return Ok(result);
    }

    /// <summary>Returns recent unread contact submissions for the notification bell.</summary>
    [HttpGet("unread")]
    public async Task<ActionResult<List<ContactResponse>>> GetUnread()
    {
        var result = await _contactServices.GetUnreadAsync();
        return Ok(result);
    }

    /// <summary>Submits a new contact form.</summary>
    [HttpPost]
    [ValidationFilter(typeof(CreateContactRequest))]
    public async Task<ActionResult<ContactResponse>> CreateContact([FromBody] CreateContactRequest request)
    {
        var result = await _contactServices.CreateContactAsync(request);
        return Ok(result);
    }

    /// <summary>Marks a contact submission as read.</summary>
    /// <param name="id">ID of the submission.</param>
    [HttpPut("{id}/read")]
    public async Task<ActionResult<ContactResponse>> MarkAsRead(string id)
    {
        var result = await _contactServices.MarkAsReadAsync(id);
        return Ok(result);
    }

    /// <summary>Deletes one or more contact submissions.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteContacts([FromQuery] List<string> ids)
    {
        await _contactServices.DeleteContactAsync(ids);
        return NoContent();
    }
}
