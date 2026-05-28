using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Category.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for category CRUD and slug-based lookup.</summary>
[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly CategoryServices _articleServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public CategoryController(ILogger<CategoryController> logger, CategoryServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }

    /// <summary>Creates a new category.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateCategoryRequest))]
    public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var category = await _articleServices.CreateCategoryAsync(request);
        return Ok(category);
    }

    /// <summary>Updates an existing category.</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateCategoryRequest))]
    public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request)
    {
        var category = await _articleServices.UpdateCategoryAsync(request);
        return Ok(category);
    }

    /// <summary>Deletes one or more categories by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteCategory([FromQuery] List<string> ids)
    {
        await _articleServices.DeleteCategoryAsync(ids);
        return NoContent();
    }

    /// <summary>Returns all categories.</summary>
    [HttpGet("")]
    public async Task<ActionResult> GetCategory()
    {
        var categories = await _articleServices.GetCategoryAsync();
        return Ok(categories);
    }

    /// <summary>Returns a single category by ID, or 404 if not found.</summary>
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

    /// <summary>Returns a single category by slug, or 404 if not found.</summary>
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
