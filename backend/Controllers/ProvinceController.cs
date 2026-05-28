using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>
/// Exposes read-only province data for the Vietnam Map feature.
/// Provinces are seeded on startup and not modifiable via API.
/// </summary>
[ApiController]
[Route("api/provinces")]
public class ProvinceController : ControllerBase
{
    private readonly ILogger<ProvinceController> _logger;
    private readonly TripServices _tripServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="tripServices">Trip and province business logic.</param>
    public ProvinceController(ILogger<ProvinceController> logger, TripServices tripServices)
    {
        _logger = logger;
        _tripServices = tripServices;
    }

    /// <summary>
    /// Returns all 63 provinces ordered by name, each with its trip count.
    /// </summary>
    /// <returns>A list of all provinces.</returns>
    [HttpGet]
    public async Task<ActionResult> GetProvinces()
    {
        var result = await _tripServices.GetProvincesWithTripCountAsync();
        return Ok(result);
    }
}
