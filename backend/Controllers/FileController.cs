using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.Files.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for file metadata CRUD and paginated listing.</summary>
[ApiController]
[Route("api/files")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly FileServices _fileServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public FileController(ILogger<FileController> logger, FileServices fileServices)
    {
        _logger = logger;
        _fileServices = fileServices;
    }

    /// <summary>Registers a new file record. The Cloudinary URL must be unique.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateFileRequest))]
    public async Task<ActionResult> CreateFile([FromBody] CreateFileRequest request)
    {
        var result = await _fileServices.CreateFileAsync(request);
        return Ok(result);
    }

    /// <summary>Updates a file's display metadata (name, title, description).</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateFileRequest))]
    public async Task<ActionResult> UpdateFile([FromBody] UpdateFileRequest request)
    {
        var result = await _fileServices.UpdateFileAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more file records by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteFile([FromQuery] List<string> ids)
    {
        await _fileServices.DeleteFileAsync(ids);
        return NoContent();
    }

    /// <summary>Returns a single file by ID, or 404 if not found.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var result = await _fileServices.GetFileByIdAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>Returns a paginated list of all files ordered by ID descending.</summary>
    [HttpGet("")]
    public async Task<ActionResult> GetPagination([FromQuery] PaginationRequest request)
    {
        var result = await _fileServices.GetPaginationFileAsync(request);
        return Ok(result);
    }
}
