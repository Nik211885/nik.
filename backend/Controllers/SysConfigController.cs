using backend.Services;
using backend.Services.Internals;
using backend.ViewModels.Configs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for system configuration key-value management.</summary>
[Route("api/config")]
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
    public async Task<ActionResult> GetConfig()
    {
        var response = await _configServices.GetConfigsAsync();
        return Ok(response);
    }

    /// <summary>Creates a new configuration entry.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateConfig([FromBody] CreateConfigRequest request)
    {
        await _configServices.CreateConfigAsync(request);
        return NoContent();
    }

    /// <summary>Updates the key and value of a specific configuration entry.</summary>
    [HttpPut("update")]
    public async Task<IActionResult> UpdateConfig([FromQuery] string id, [FromBody] CreateConfigRequest request)
    {
        await _configServices.UpdateConfigSpecificByIdAsync(id, request);
        return NoContent();
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
    public async Task<ActionResult> GetConfigById(string id)
    {
        var response = await _configServices.GetConfigByIdAsync(id);
        return Ok(response);
    }
}
