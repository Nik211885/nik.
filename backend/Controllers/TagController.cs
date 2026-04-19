using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly TagServices _tagServices;

    public TagController(ILogger<TagController> logger, TagServices tagServices)
    {
        _logger = logger;
        _tagServices = tagServices;
    }
}
