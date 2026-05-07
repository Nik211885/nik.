using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Albums.Requests;
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

    [HttpPost("create")]
    [ValidationFilter(typeof(CreateAlbumRequest))]
    public async Task<ActionResult> CreateAlbum([FromBody] CreateAlbumRequest request)
    {
        var result = await _albumServices.CreateAlbumAsync(request);
        return Ok(result);
    }

    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateAlbumRequest))]
    public async Task<ActionResult> UpdateAlbum([FromBody] UpdateAlbumRequest request)
    {
        var result = await _albumServices.UpdateAlbumAsync(request);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteAlbum([AsParameters] List<string> ids)
    {
        await _albumServices.DeleteAlbumAsync(ids);
        return NoContent();
    }

    [HttpPost("files/add")]
    [ValidationFilter(typeof(AddFilesToAlbumRequest))]
    public async Task<ActionResult> AddFiles([FromBody] AddFilesToAlbumRequest request)
    {
        var result = await _albumServices.AddFilesToAlbumAsync(request);
        return Ok(result);
    }

    [HttpDelete("files/remove")]
    [ValidationFilter(typeof(RemoveFilesFromAlbumRequest))]
    public async Task<ActionResult> RemoveFiles([FromBody] RemoveFilesFromAlbumRequest request)
    {
        await _albumServices.RemoveFilesFromAlbumAsync(request);
        return NoContent();
    }

    [HttpGet("{albumId}/files")]
    public async Task<ActionResult> GetFiles(string albumId)
    {
        var result = await _albumServices.GetAlbumFilesAsync(albumId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumByIdAsync(id, tree);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumBySlugAsync(slug, tree);
        return Ok(result);
    }

    [HttpGet("parents")]
    public async Task<IActionResult> GetParent([FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumParentAsync(tree);
        return Ok(result);
    }

    [HttpGet("childrens/{parentId}")]
    public async Task<IActionResult> GetChildren(string parentId, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumChildrenAsync(parentId, tree);
        return Ok(result);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var result = await _albumServices.BuildAlbumTreeAsync();
        return Ok(result);
    }
}
