using backend.Services;
using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/albums")]
public class AlbumController : ControllerBase
{
    private readonly ILogger<AlbumController> _logger;
    private readonly AlbumServices _albumServices;

    public AlbumController(ILogger<AlbumController> logger, AlbumServices albumServices)
    {
        _logger = logger;
        _albumServices = albumServices;
    }
}
