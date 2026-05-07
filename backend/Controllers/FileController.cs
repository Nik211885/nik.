using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.Files.Requests;
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

    [HttpPost("create")]
    [ValidationFilter(typeof(CreateFileRequest))]
    public async Task<ActionResult> CreateFile([FromBody] CreateFileRequest request)
    {
        var result = await _fileServices.CreateFileAsync(request);
        return Ok(result);
    }

    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateFileRequest))]
    public async Task<ActionResult> UpdateFile([FromBody] UpdateFileRequest request)
    {
        var result = await _fileServices.UpdateFileAsync(request);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteFile([AsParameters] List<string> ids)
    {
        await _fileServices.DeleteFileAsync(ids);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var result = await _fileServices.GetFileByIdAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("")]
    public async Task<ActionResult> GetPagination([AsParameters] PaginationRequest request)
    {
        var result = await _fileServices.GetPaginationFileAsync(request);
        return Ok(result);
    }
}
