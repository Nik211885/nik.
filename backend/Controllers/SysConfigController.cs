using backend.Services;
using backend.Services.Internals;
using backend.ViewModels.Configs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/config")]
[ApiController]
public class SysConfigController : ControllerBase
{
    private readonly ILogger<SysConfigController> _logger;
    private readonly SysConfigServices _configServices;

    public SysConfigController(ILogger<SysConfigController> logger, SysConfigServices configServices)
    {
        _logger = logger;
        _configServices = configServices;
    }

    [HttpGet("")]
    public async Task<ActionResult> GetConfig()
    {
        var response = await _configServices.GetConfigsAsync();
        return Ok(response);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateConfig([FromBody] CreateConfigRequest request)
    {
        await _configServices.CreateConfigAsync(request);
        return NoContent();
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateConfig([FromQuery] string id, [FromBody]CreateConfigRequest request)
    {
        await _configServices.UpdateConfigSpecificByIdAsync(id, request);
        return NoContent();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteConfig([AsParameters] string[] ids)
    {
        await _configServices.DeleteConfigByIdsAsync(ids.ToList());
        return NoContent();
    }

    [HttpGet("specific-by")]
    public async Task<ActionResult> GetConfigById(string id)
    {
        var response = await _configServices.GetConfigByIdAsync(id);
        return Ok(response);
    }
}
