using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Tag.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace backend.Controllers;

/// <summary>Endpoints for tag CRUD and slug-based lookup.</summary>
[ApiController]
[Route("api/tags")]
public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly TagServices _tagServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public TagController(ILogger<TagController> logger, TagServices tagServices)
    {
        _logger = logger;
        _tagServices = tagServices;
    }

    /// <summary>Creates a new tag.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateTagRequest))]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest request)
    {
        var result = await _tagServices.CreateTagAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing tag.</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateTagRequest))]
    public async Task<ActionResult> UpdateTag([FromBody] UpdateTagRequest request)
    {
        var result = await _tagServices.UpdateTagAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more tags by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteAsync([FromQuery] List<string> ids)
    {
        await _tagServices.DeleteTagAsync(ids);
        return NoContent();
    }

    /// <summary>Returns all tags.</summary>
    [HttpGet("")]
    public async Task<ActionResult> GetTag()
    {
        var result = await _tagServices.GetTagAsync();
        return Ok(result);
    }

    /// <summary>Returns a single tag by ID, or 404 if not found.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTagById(string id)
    {
        var result = await _tagServices.GetTagByIdAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    /// <summary>Returns a single tag by slug, or 404 if not found.</summary>
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult> GetTagBySlug(string slug)
    {
        var result = await _tagServices.GetTagBySlugAsync(slug);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
