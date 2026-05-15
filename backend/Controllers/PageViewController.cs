using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.PageView.Requests;
using backend.ViewModels.PageView.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for page-view tracking and statistics.</summary>
[ApiController]
[Route("api/page-views")]
public class PageViewController : ControllerBase
{
    private readonly ILogger<PageViewController> _logger;
    private readonly PageViewServices _pageViewServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public PageViewController(ILogger<PageViewController> logger, PageViewServices pageViewServices)
    {
        _logger = logger;
        _pageViewServices = pageViewServices;
    }

    /// <summary>
    /// Records a page-view event sent by the public frontend on every route change.
    /// No authentication required.
    /// </summary>
    [HttpPost]
    [ValidationFilter(typeof(CreatePageViewRequest))]
    public async Task<ActionResult> Record([FromBody] CreatePageViewRequest request)
    {
        await _pageViewServices.RecordAsync(request);
        return NoContent();
    }

    /// <summary>Returns a paginated list of raw page-view records for the admin table.</summary>
    [HttpGet]
    public async Task<ActionResult<PaginationItem<PageViewResponse>>> GetAll([FromQuery] PaginationRequest request)
    {
        var result = await _pageViewServices.GetPaginationAsync(request);
        return Ok(result);
    }

    /// <summary>Returns aggregated statistics for the admin chart.</summary>
    /// <param name="period"><c>week</c>, <c>month</c>, or <c>year</c>.</param>
    [HttpGet("stats")]
    public async Task<ActionResult<PageViewStatsResponse>> GetStats([FromQuery] string period = "week")
    {
        var result = await _pageViewServices.GetStatsAsync(period);
        return Ok(result);
    }
}
