using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Contact.Requests;
using backend.ViewModels.Contact.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for contact form submissions.</summary>
public class ContactServices
{
    private readonly ILogger<ContactServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public ContactServices(ILogger<ContactServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Saves a new contact form submission.
    /// </summary>
    /// <param name="request">The contact form payload.</param>
    /// <returns>The saved contact response.</returns>
    public async Task<ContactResponse> CreateContactAsync(CreateContactRequest request)
    {
        var contact = new Entities.Contact
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedDate = DateTimeOffset.UtcNow,
            IsRead = false
        };

        _dbContext.Contacts.Add(contact);
        await _dbContext.SaveChangesAsync();

        return ToResponse(contact);
    }

    /// <summary>
    /// Returns a paginated list of contact submissions, newest first.
    /// </summary>
    /// <param name="request">Pagination parameters.</param>
    public async Task<PaginationItem<ContactResponse>> GetPaginationAsync(PaginationRequest request)
    {
        return await _dbContext.Contacts
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedDate)
            .Select(c => new ContactResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Subject = c.Subject,
                Message = c.Message,
                CreatedDate = c.CreatedDate,
                IsRead = c.IsRead
            })
            .PaginationItemAsync(request);
    }

    /// <summary>
    /// Returns the most recent unread contact submissions for the notification bell.
    /// </summary>
    /// <param name="count">Maximum items to return. Defaults to 20.</param>
    public async Task<List<ContactResponse>> GetUnreadAsync(int count = 20)
    {
        return await _dbContext.Contacts
            .Where(c => !c.IsRead)
            .OrderByDescending(c => c.CreatedDate)
            .Take(count)
            .Select(c => new ContactResponse
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Subject = c.Subject,
                Message = c.Message,
                CreatedDate = c.CreatedDate,
                IsRead = c.IsRead
            })
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Marks a contact submission as read.
    /// </summary>
    /// <param name="id">ID of the contact submission.</param>
    /// <exception cref="NotFoundException">Thrown when no submission with the given ID exists.</exception>
    public async Task<ContactResponse> MarkAsReadAsync(string id)
    {
        var contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException();

        contact.IsRead = true;
        await _dbContext.SaveChangesAsync();

        return ToResponse(contact);
    }

    /// <summary>
    /// Deletes one or more contact submissions by ID.
    /// </summary>
    /// <param name="ids">IDs of the submissions to delete.</param>
    public async Task DeleteContactAsync(List<string> ids)
    {
        await _dbContext.Contacts
            .Where(c => ids.Contains(c.Id))
            .ExecuteDeleteAsync();
    }

    private static ContactResponse ToResponse(Entities.Contact c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Subject = c.Subject,
        Message = c.Message,
        CreatedDate = c.CreatedDate,
        IsRead = c.IsRead
    };
}
