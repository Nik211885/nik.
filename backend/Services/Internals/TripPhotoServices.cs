using backend.Data;
using backend.Exceptions;
using backend.ViewModels.VietnamMap.Requests;
using backend.ViewModels.VietnamMap.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Business logic for managing photos attached to travel trips.
/// </summary>
public class TripPhotoServices
{
    private readonly ILogger<TripPhotoServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="dbContext">The application database context.</param>
    public TripPhotoServices(ILogger<TripPhotoServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns all photos for the specified trip, ordered by display order.
    /// </summary>
    /// <param name="tripId">The trip identifier.</param>
    /// <returns>An ordered list of trip photos.</returns>
    public async Task<List<TripPhotoResponse>> GetByTripAsync(string tripId)
    {
        return await _dbContext.TripPhotos
            .AsNoTracking()
            .Where(p => p.TripId == tripId)
            .OrderBy(p => p.Order)
            .Select(p => p.ToTripPhotoResponse())
            .ToListAsync();
    }

    /// <summary>
    /// Attaches a new photo to the specified trip. Auto-assigns order as last position.
    /// </summary>
    /// <param name="request">Photo creation payload.</param>
    /// <returns>The created photo response.</returns>
    /// <exception cref="NotFoundException">Thrown when the specified trip does not exist.</exception>
    public async Task<TripPhotoResponse> AddPhotoAsync(AddTripPhotoRequest request)
    {
        if (!await _dbContext.Trips.AnyAsync(t => t.Id == request.TripId))
            throw new NotFoundException(ApplicationMessage.TripNotFound);

        if (request.Order == 0)
        {
            var maxOrder = await _dbContext.TripPhotos
                .Where(p => p.TripId == request.TripId)
                .Select(p => (int?)p.Order)
                .MaxAsync() ?? -1;
            request.Order = maxOrder + 1;
        }

        var photo = request.ToTripPhoto();
        _dbContext.TripPhotos.Add(photo);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Added photo {PhotoId} to trip {TripId}", photo.Id, photo.TripId);
        return photo.ToTripPhotoResponse();
    }

    /// <summary>
    /// Deletes one or more photos by their IDs.
    /// </summary>
    /// <param name="ids">List of photo IDs to delete.</param>
    public async Task DeletePhotosAsync(List<string> ids)
    {
        await _dbContext.TripPhotos
            .Where(p => ids.Contains(p.Id))
            .ExecuteDeleteAsync();

        _logger.LogInformation("Deleted {Count} trip photo(s)", ids.Count);
    }
}
