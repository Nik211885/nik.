using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.VietnamMap.Requests;
using backend.ViewModels.VietnamMap.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>
/// Manages photos attached to travel trips (Vietnam Map feature).
/// </summary>
[ApiController]
[Route("api/trip-photos")]
public class TripPhotoController : ControllerBase
{
    private readonly ILogger<TripPhotoController> _logger;
    private readonly TripPhotoServices _tripPhotoServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="tripPhotoServices">Trip photo business logic service.</param>
    public TripPhotoController(ILogger<TripPhotoController> logger, TripPhotoServices tripPhotoServices)
    {
        _logger = logger;
        _tripPhotoServices = tripPhotoServices;
    }

    /// <summary>Returns all photos for the specified trip, ordered by display order.</summary>
    /// <param name="tripId">The trip identifier.</param>
    /// <returns>Ordered list of photos.</returns>
    [HttpGet]
    public async Task<ActionResult<List<TripPhotoResponse>>> GetByTrip([FromQuery] string tripId)
    {
        var photos = await _tripPhotoServices.GetByTripAsync(tripId);
        return Ok(photos);
    }

    /// <summary>Attaches a new photo to a trip.</summary>
    /// <param name="request">Photo creation payload.</param>
    /// <returns>The created photo.</returns>
    [HttpPost("create")]
    [ValidationFilter(typeof(AddTripPhotoRequest))]
    public async Task<ActionResult<TripPhotoResponse>> Create([FromBody] AddTripPhotoRequest request)
    {
        var result = await _tripPhotoServices.AddPhotoAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more photos by their IDs.</summary>
    /// <param name="ids">List of photo IDs to delete.</param>
    /// <returns>204 No Content.</returns>
    [HttpDelete("delete")]
    public async Task<ActionResult> Delete([FromQuery] List<string> ids)
    {
        await _tripPhotoServices.DeletePhotosAsync(ids);
        return NoContent();
    }
}
