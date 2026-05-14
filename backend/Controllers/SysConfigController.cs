using backend.Services.Internals;
using backend.ViewModels.Configs.Requests;
using backend.ViewModels.Configs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for system configuration key-value management.</summary>
[Route("api/sys-configs")]
[ApiController]
public class SysConfigController : ControllerBase
{
    private readonly ILogger<SysConfigController> _logger;
    private readonly SysConfigServices _configServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public SysConfigController(ILogger<SysConfigController> logger, SysConfigServices configServices)
    {
        _logger = logger;
        _configServices = configServices;
    }

    /// <summary>Returns all configuration entries.</summary>
    [HttpGet("")]
    public async Task<ActionResult<IReadOnlyCollection<ConfigResponse>>> GetConfig()
    {
        var response = await _configServices.GetConfigsAsync();
        return Ok(response);
    }

    /// <summary>Creates a new configuration entry and returns it.</summary>
    [HttpPost("create")]
    public async Task<ActionResult<ConfigResponse>> CreateConfig([FromBody] CreateConfigRequest request)
    {
        var result = await _configServices.CreateConfigAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing configuration entry and returns the updated record.</summary>
    [HttpPut("update")]
    public async Task<ActionResult<ConfigResponse>> UpdateConfig([FromBody] UpdateConfigRequest request)
    {
        var result = await _configServices.UpdateConfigAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more configuration entries by ID.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteConfig([AsParameters] string[] ids)
    {
        await _configServices.DeleteConfigByIdsAsync(ids.ToList());
        return NoContent();
    }

    /// <summary>Returns a single configuration entry by ID.</summary>
    [HttpGet("specific-by")]
    public async Task<ActionResult<ConfigResponse>> GetConfigById([FromQuery] string id)
    {
        var response = await _configServices.GetConfigByIdAsync(id);
        return Ok(response);
    }

    /// <summary>Returns all SysConfig entries plus article and category archive stats. No authentication required.</summary>
    [HttpGet("~/public-api/config")]
    public async Task<ActionResult> GetPublicConfig()
    {
        var result = await _configServices.GetPublicConfigAsync();
        return Ok(result);
    }
}
