using backend.Services;
using backend.Services.Internals;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly FileServices _fileServices;

    public FileController(ILogger<FileController> logger, FileServices fileServices)
    {
        _logger = logger;
        _fileServices = fileServices;
    }
}
