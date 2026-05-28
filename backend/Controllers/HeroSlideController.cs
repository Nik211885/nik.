using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.HeroSlides.Requests;
using backend.ViewModels.HeroSlides.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for hero carousel slide management.</summary>
[ApiController]
[Route("api/hero-slides")]
public class HeroSlideController : ControllerBase
{
    private readonly ILogger<HeroSlideController> _logger;
    private readonly HeroSlideServices _heroSlideServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public HeroSlideController(ILogger<HeroSlideController> logger, HeroSlideServices heroSlideServices)
    {
        _logger = logger;
        _heroSlideServices = heroSlideServices;
    }

    /// <summary>Returns all slides for the admin panel.</summary>
    [HttpGet]
    public async Task<ActionResult<List<HeroSlideResponse>>> GetAll()
    {
        var result = await _heroSlideServices.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Returns only active slides for the public carousel. No authentication required.</summary>
    [HttpGet("~/public-api/hero-slides")]
    public async Task<ActionResult<List<HeroSlideResponse>>> GetPublic()
    {
        var result = await _heroSlideServices.GetPublicAsync();
        return Ok(result);
    }

    /// <summary>Creates a new hero slide.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateHeroSlideRequest))]
    public async Task<ActionResult<HeroSlideResponse>> Create([FromBody] CreateHeroSlideRequest request)
    {
        var result = await _heroSlideServices.CreateAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing hero slide.</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateHeroSlideRequest))]
    public async Task<ActionResult<HeroSlideResponse>> Update([FromBody] UpdateHeroSlideRequest request)
    {
        var result = await _heroSlideServices.UpdateAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more slides by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] string[] ids)
    {
        await _heroSlideServices.DeleteAsync(ids.ToList());
        return NoContent();
    }
}
