using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Articles.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for article CRUD, slug-based lookup, and paginated listing with filters.</summary>
[ApiController]
[Route("api/articles")]
public class ArticleController : ControllerBase
{
    private readonly ILogger<ArticleController> _logger;
    private readonly ArticleServices _articleServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public ArticleController(ILogger<ArticleController> logger, ArticleServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }

    /// <summary>Creates a new article for the authenticated user.</summary>
    [HttpPost("create")]
    [ValidationFilter(typeof(CreateArticleRequest))]
    public async Task<ActionResult> CreateArticle([FromBody] CreateArticleRequest request)
    {
        var result = await _articleServices.CreateArticleAsync(request);
        return Ok(result);
    }

    /// <summary>Updates an existing article and synchronises its tag/category associations.</summary>
    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateArticleRequest))]
    public async Task<ActionResult> UpdateArticle([FromBody] UpdateArticleRequest request)
    {
        var result = await _articleServices.UpdateArticleAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more articles by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteArticle([FromQuery] List<string> ids)
    {
        await _articleServices.DeleteArticleAsync(ids);
        return NoContent();
    }

    /// <summary>Returns a single article by ID, or 404 if not found.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var result = await _articleServices.GetArticleByIdAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>Returns a single article by slug and increments its view count, or 404 if not found.</summary>
    [HttpGet("slug/{**slug}")]
    public async Task<ActionResult> GetBySlug(string slug)
    {
        var result = await _articleServices.GetArticleBySlugAsync(slug);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    /// <summary>Returns the top 12 articles ranked by total engagement (views + likes + hearts + comments).</summary>
    [HttpGet("top")]
    public async Task<ActionResult> GetTopArticles()
    {
        var result = await _articleServices.GetTopArticlesAsync();
        return Ok(result);
    }

    /// <summary>Returns a paginated list of articles, optionally filtered by category slug, tag slug, or keyword.</summary>
    [HttpGet("")]
    public async Task<ActionResult> GetPagination([FromQuery] GetArticlesPaginationRequest request)
    {
        var result = await _articleServices.GetPaginationArticleAsync(request);
        return Ok(result);
    }
}
