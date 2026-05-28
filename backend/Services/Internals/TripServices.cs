using backend.Data;
using backend.Exceptions;
using backend.ViewModels.VietnamMap.Requests;
using backend.ViewModels.VietnamMap.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Business logic for managing travel trips and provinces in the Vietnam Map feature.
/// </summary>
public class TripServices
{
    private readonly ILogger<TripServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="dbContext">The application database context.</param>
    public TripServices(ILogger<TripServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns all 63 provinces ordered by name, each including the number of trips recorded.
    /// </summary>
    /// <returns>A list of all provinces with their trip counts.</returns>
    public async Task<List<ProvinceResponse>> GetProvincesWithTripCountAsync()
    {
        return await _dbContext.Provinces
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProvinceResponse
            {
                Id        = p.Id,
                Name      = p.Name,
                Code      = p.Code,
                TripCount = p.Trips.Count(),
            })
            .ToListAsync();
    }

    /// <summary>
    /// Returns all trips for the specified province, ordered newest date first.
    /// </summary>
    /// <param name="provinceId">The province identifier.</param>
    /// <returns>A list of trips for the province.</returns>
    /// <exception cref="NotFoundException">Thrown when no province with the given ID exists.</exception>
    public async Task<List<TripResponse>> GetTripsByProvinceAsync(string provinceId)
    {
        if (!await _dbContext.Provinces.AnyAsync(p => p.Id == provinceId))
            throw new NotFoundException(ApplicationMessage.NotFound);

        return await _dbContext.Trips
            .AsNoTracking()
            .Where(t => t.ProvinceId == provinceId)
            .OrderByDescending(t => t.Date)
            .ToTripResponses()
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new trip for the specified province.
    /// </summary>
    /// <param name="request">Trip creation payload.</param>
    /// <returns>The created trip response.</returns>
    /// <exception cref="NotFoundException">Thrown when the specified province does not exist.</exception>
    public async Task<TripResponse> CreateTripAsync(CreateTripRequest request)
    {
        if (!await _dbContext.Provinces.AnyAsync(p => p.Id == request.ProvinceId))
            throw new NotFoundException(ApplicationMessage.NotFound);

        var trip = request.ToTrip();
        _dbContext.Trips.Add(trip);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created trip {TripId} for province {ProvinceId}", trip.Id, trip.ProvinceId);
        return trip.ToTripResponse();
    }

    /// <summary>
    /// Updates the title, date, and story of an existing trip.
    /// </summary>
    /// <param name="request">Trip update payload.</param>
    /// <returns>The updated trip response.</returns>
    /// <exception cref="NotFoundException">Thrown when no trip with the given ID exists.</exception>
    public async Task<TripResponse> UpdateTripAsync(UpdateTripRequest request)
    {
        var trip = await _dbContext.Trips.FirstOrDefaultAsync(t => t.Id == request.Id)
            ?? throw new NotFoundException(ApplicationMessage.TripNotFound);

        trip.Title       = request.Title;
        trip.Date        = request.Date;
        trip.Story       = request.Story;
        trip.UpdatedDate = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Updated trip {TripId}", trip.Id);
        return trip.ToTripResponse();
    }

    /// <summary>
    /// Deletes the trip with the specified ID.
    /// </summary>
    /// <param name="id">The trip identifier.</param>
    public async Task DeleteTripAsync(string id)
    {
        await _dbContext.Trips
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();

        _logger.LogInformation("Deleted trip {TripId}", id);
    }
}
