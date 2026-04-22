using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Category.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly CategoryServices _articleServices;

    public CategoryController(ILogger<CategoryController> logger, CategoryServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateCategoryRequest))]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = await _articleServices.CreateCategoryAsync(request);
        return Ok(category);
    }
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateCategoryRequest))]
    public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request) 
    {
        var category = await _articleServices.UpdateCategoryAsync(request);
        return Ok(category);
    }
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteCategory([FromBody] List<string> ids)
    {
        await _articleServices.DeleteCategoryAsync(ids);
        return NoContent();
    }
    [HttpGet("")]
    public async Task<ActionResult> GetCategory()
    {
        var categories = await _articleServices.GetCategoryAsync();
        return Ok(categories);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var category = await _articleServices.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
     }
     [HttpGet("slug/{slug}")]
     public async Task<ActionResult> GetBySlug(string slug)
     {
        var category = await _articleServices.GetBySlugAsync(slug);
        if (category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

}
