using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Articles.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/articles")]
public class ArticleController : ControllerBase
{
    private readonly ILogger<ArticleController> _logger;
    private readonly ArticleServices _articleServices;

    public ArticleController(ILogger<ArticleController> logger, ArticleServices articleServices)
    {
        _logger = logger;
        _articleServices = articleServices;
    }

    [HttpPost("create")]
    [ValidationFilter(typeof(CreateArticleRequest))]
    public async Task<ActionResult> CreateArticle([FromBody] CreateArticleRequest request)
    {
        var result = await _articleServices.CreateArticleAsync(request);
        return Ok(result);
    }

    [HttpPut("update")]
    [ValidationFilter(typeof(UpdateArticleRequest))]
    public async Task<ActionResult> UpdateArticle([FromBody] UpdateArticleRequest request)
    {
        var result = await _articleServices.UpdateArticleAsync(request);
        return Ok(result);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteArticle([AsParameters] List<string> ids)
    {
        await _articleServices.DeleteArticleAsync(ids);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var result = await _articleServices.GetArticleByIdAsync(id);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult> GetBySlug(string slug)
    {
        var result = await _articleServices.GetArticleBySlugAsync(slug);
        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpGet("")]
    public async Task<ActionResult> GetPagination([AsParameters] GetArticlesPaginationRequest request)
    {
        var result = await _articleServices.GetPaginationArticleAsync(request);
        return Ok(result);
    }
}
