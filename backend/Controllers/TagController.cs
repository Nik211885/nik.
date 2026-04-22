using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Tag.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateTagRequest))]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest request)
    {
        var result = await _tagServices.CreateTagAsync(request);
        return Ok(result);
    }
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateTagRequest))]
    public async Task<ActionResult> UpdateTag([FromBody] UpdateTagRequest request)
    {
        var result = await _tagServices.UpdateTagAsync(request);
        return Ok(result);
    }
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteAsync([AsParameters] List<string> ids)
    {
        await _tagServices.DeleteTagAsync(ids);
        return NoContent();
    }
    [HttpGet("")]
    public async Task<ActionResult> GetTag()
    {
        var result = await _tagServices.GetTagAsync();
        return Ok(result);
    }
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
