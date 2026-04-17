using backend.Services;
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
}
