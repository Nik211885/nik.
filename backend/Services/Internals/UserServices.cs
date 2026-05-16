using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Users.Requests;
using backend.ViewModels.Users.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for user management.</summary>
public class UserServices
{
    private readonly ILogger<UserServices> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public UserServices(ILogger<UserServices> logger, ApplicationDbContext context, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _context = context;
        _httpContext = httpContext;
    }

    /// <summary>
    /// Returns all registered users.
    /// </summary>
    /// <returns>A list of <see cref="UserResponse"/> objects.</returns>
    public async Task<List<UserResponse>> GetUsersAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToUserResponses()
            .ToListAsync();
    }

    /// <summary>
    /// Returns a single user by ID.
    /// </summary>
    /// <param name="id">The user ID.</param>
    /// <returns>The matching <see cref="UserResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when no user with the given ID exists.</exception>
    public async Task<UserResponse> GetUserAsync(string id)
    {
        return await _context.Users
            .AsNoTracking()
            .ToUserResponses()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException();
    }

    /// <summary>
    /// Updates the authenticated user's profile fields.
    /// </summary>
    /// <param name="request">Updated profile data.</param>
    /// <returns>The updated <see cref="UserResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the current user cannot be found.</exception>
    public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest request)
    {
        var userId = _httpContext.HttpContext!.GetUserId();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException();

        user.UserName = request.UserName;
        user.Email = request.Email ?? user.Email;
        user.Phone = request.Phone ?? user.Phone;
        user.Bio = request.Bio;
        user.Slug = request.UserName.ToSlug();
        user.UpdatedDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return user.ToUserResponse();
    }

    /// <summary>
    /// Updates a specific user's profile fields by ID (admin use).
    /// </summary>
    /// <param name="id">The target user ID.</param>
    /// <param name="request">Updated profile data.</param>
    /// <returns>The updated <see cref="UserResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when no user with the given ID exists.</exception>
    public async Task<UserResponse> UpdateUserByIdAsync(string id, UpdateUserRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException();

        user.UserName = request.UserName;
        user.Email = request.Email ?? user.Email;
        user.Phone = request.Phone ?? user.Phone;
        user.Bio = request.Bio;
        user.Slug = request.UserName.ToSlug();
        user.UpdatedDate = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return user.ToUserResponse();
    }
}
