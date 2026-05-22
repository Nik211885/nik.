using backend.Entities;
using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.WallMessages.Requests;
using backend.ViewModels.WallMessages.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for wall message submissions and moderation.</summary>
[ApiController]
[Route("api/wall-messages")]
public class WallMessageController : ControllerBase
{
    private readonly ILogger<WallMessageController> _logger;
    private readonly WallMessageServices _wallMessageServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public WallMessageController(ILogger<WallMessageController> logger, WallMessageServices wallMessageServices)
    {
        _logger = logger;
        _wallMessageServices = wallMessageServices;
    }

    /// <summary>Returns all approved wall messages for the public wall page.</summary>
    [HttpGet("~/public-api/wall-messages")]
    public async Task<ActionResult<List<WallMessageResponse>>> GetApproved()
    {
        var result = await _wallMessageServices.GetApprovedAsync();
        return Ok(result);
    }

    /// <summary>Submits a new wall message. Claude moderates it automatically.</summary>
    [HttpPost("~/public-api/wall-messages")]
    [ValidationFilter(typeof(CreateWallMessageRequest))]
    public async Task<ActionResult<WallMessageResponse>> Create([FromBody] CreateWallMessageRequest request)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _wallMessageServices.CreateAsync(request, ip);
        return Ok(result);
    }

    /// <summary>Toggles a reaction from a device on an approved wall message.</summary>
    /// <param name="id">ID of the wall message.</param>
    /// <param name="request">Device identifier payload.</param>
    [HttpPost("~/public-api/wall-messages/{id}/react")]
    [ValidationFilter(typeof(ReactWallMessageRequest))]
    public async Task<ActionResult<ReactWallMessageResponse>> React(string id, [FromBody] ReactWallMessageRequest request)
    {
        var result = await _wallMessageServices.ReactAsync(id, request.DeviceId);
        return Ok(result);
    }

    /// <summary>Returns a paginated list of all wall messages for the admin panel.</summary>
    [HttpGet]
    public async Task<ActionResult<PaginationItem<AdminWallMessageResponse>>> GetAll(
        [FromQuery] PaginationRequest request,
        [FromQuery] WallMessageStatus? status = null)
    {
        var result = await _wallMessageServices.GetPaginationAsync(request, status);
        return Ok(result);
    }

    /// <summary>Updates the moderation status of a wall message.</summary>
    /// <param name="id">ID of the message.</param>
    /// <param name="status">New status to apply.</param>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<AdminWallMessageResponse>> UpdateStatus(string id, [FromQuery] WallMessageStatus status)
    {
        var result = await _wallMessageServices.UpdateStatusAsync(id, status);
        return Ok(result);
    }

    /// <summary>Updates the moderation status of multiple wall messages in one operation.</summary>
    [HttpPut("bulk-status")]
    public async Task<IActionResult> BulkUpdateStatus(
        [FromQuery] List<string> ids,
        [FromQuery] WallMessageStatus status)
    {
        await _wallMessageServices.BulkUpdateStatusAsync(ids, status);
        return NoContent();
    }

    /// <summary>Deletes one or more wall messages by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] List<string> ids)
    {
        await _wallMessageServices.DeleteAsync(ids);
        return NoContent();
    }
}
