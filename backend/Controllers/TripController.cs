using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.VietnamMap.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>
/// Manages travel trips for the Vietnam Map admin feature.
/// Supports CRUD operations per province.
/// </summary>
[ApiController]
[Route("api/trips")]
public class TripController : ControllerBase
{
    private readonly ILogger<TripController> _logger;
    private readonly TripServices _tripServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="tripServices">Trip and province business logic.</param>
    public TripController(ILogger<TripController> logger, TripServices tripServices)
    {
        _logger = logger;
        _tripServices = tripServices;
    }

    /// <summary>
    /// Returns all trips for the specified province, ordered newest first.
    /// </summary>
    /// <param name="provinceId">The province identifier.</param>
    /// <returns>A list of trips for the province.</returns>
    [HttpGet]
    public async Task<ActionResult> GetTrips([FromQuery] string provinceId)
    {
        var result = await _tripServices.GetTripsByProvinceAsync(provinceId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a new trip for the specified province.
    /// </summary>
    /// <param name="request">Trip creation payload.</param>
    /// <returns>The created trip.</returns>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateTripRequest))]
    public async Task<ActionResult> CreateTrip(CreateTripRequest request)
    {
        var result = await _tripServices.CreateTripAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing trip's title, date, and story.
    /// </summary>
    /// <param name="request">Trip update payload.</param>
    /// <returns>The updated trip.</returns>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateTripRequest))]
    public async Task<ActionResult> UpdateTrip(UpdateTripRequest request)
    {
        var result = await _tripServices.UpdateTripAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Deletes the trip with the specified ID.
    /// </summary>
    /// <param name="ids">The trip identifier(s) to delete.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteTrip([AsParameters] List<string> ids)
    {
        foreach (var id in ids)
            await _tripServices.DeleteTripAsync(id);
        return Ok();
    }
}
