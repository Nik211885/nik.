using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Albums.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for album management and hierarchical tree operations.</summary>
[ApiController]
[Route("api/albums")]
public class AlbumController : ControllerBase
{
    private readonly ILogger<AlbumController> _logger;
    private readonly AlbumServices _albumServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public AlbumController(ILogger<AlbumController> logger, AlbumServices albumServices)
    {
        _logger = logger;
        _albumServices = albumServices;
    }

    /// <summary>Creates a new album.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateAlbumRequest))]
    public async Task<ActionResult> CreateAlbum([FromBody] CreateAlbumRequest request)
    {
        var result = await _albumServices.CreateAlbumAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing album.</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateAlbumRequest))]
    public async Task<ActionResult> UpdateAlbum([FromBody] UpdateAlbumRequest request)
    {
        var result = await _albumServices.UpdateAlbumAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more albums by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteAlbum([FromQuery] List<string> ids)
    {
        await _albumServices.DeleteAlbumAsync(ids);
        return NoContent();
    }

    /// <summary>Sets or clears the cover image for an album.</summary>
    [HttpPut("set-cover")]
    [ValidationFilter(typeof(SetCoverRequest))]
    public async Task<ActionResult> SetCover([FromBody] SetCoverRequest request)
    {
        var result = await _albumServices.SetCoverAsync(request);
        return Ok(result);
    }

    /// <summary>Adds files to an album, skipping duplicates.</summary>
    [HttpPost("files/add")]
    [ValidationFilter(typeof(AddFilesToAlbumRequest))]
    public async Task<ActionResult> AddFiles([FromBody] AddFilesToAlbumRequest request)
    {
        var result = await _albumServices.AddFilesToAlbumAsync(request);
        return Ok(result);
    }

    /// <summary>Removes files from an album.</summary>
    [HttpDelete("files/remove")]
    [ValidationFilter(typeof(RemoveFilesFromAlbumRequest))]
    public async Task<ActionResult> RemoveFiles([FromBody] RemoveFilesFromAlbumRequest request)
    {
        await _albumServices.RemoveFilesFromAlbumAsync(request);
        return NoContent();
    }

    /// <summary>Returns all files belonging to the specified album.</summary>
    [HttpGet("{albumId}/files")]
    public async Task<ActionResult> GetFiles(string albumId)
    {
        var result = await _albumServices.GetAlbumFilesAsync(albumId);
        return Ok(result);
    }

    /// <summary>Returns a single album by ID. Pass <c>tree=true</c> to include the child hierarchy.</summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumByIdAsync(id, tree);
        return Ok(result);
    }

    /// <summary>Returns a single album by slug. Pass <c>tree=true</c> to include the child hierarchy.</summary>
    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumBySlugAsync(slug, tree);
        return Ok(result);
    }

    /// <summary>Returns all albums as a flat list.</summary>
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _albumServices.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Returns all root-level albums. Pass <c>tree=true</c> to include descendants.</summary>
    [HttpGet("parents")]
    public async Task<IActionResult> GetParent([FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumParentAsync(tree);
        return Ok(result);
    }

    /// <summary>Returns direct children of the specified parent album. Pass <c>tree=true</c> to include descendants.</summary>
    [HttpGet("childrens/{parentId}")]
    public async Task<IActionResult> GetChildren(string parentId, [FromQuery] bool tree = false)
    {
        var result = await _albumServices.GetAlbumChildrenAsync(parentId, tree);
        return Ok(result);
    }

    /// <summary>Returns the full album hierarchy with all descendants nested.</summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var result = await _albumServices.BuildAlbumTreeAsync();
        return Ok(result);
    }
}
