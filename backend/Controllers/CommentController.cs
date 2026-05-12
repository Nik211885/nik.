using backend.Pipes.Filter;
using backend.Services.Internals;
using backend.ViewModels.Comment.Requests;
using backend.ViewModels.Comment.Responses;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for article comment creation, listing, and deletion.</summary>
[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly CommentServices _commentServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public CommentController(ILogger<CommentController> logger, CommentServices commentServices)
    {
        _logger = logger;
        _commentServices = commentServices;
    }

    /// <summary>Returns comments for the specified article.</summary>
    /// <param name="articleId">ID of the article whose comments to retrieve.</param>
    [HttpGet]
    public async Task<ActionResult<List<CommentResponse>>> GetComments([FromQuery] string articleId)
    {
        var result = await _commentServices.GetByArticleAsync(articleId);
        return Ok(result);
    }

    /// <summary>Creates a new comment.</summary>
    [HttpPost]
    [ValidationFilter(typeof(CreateCommentRequest))]
    public async Task<ActionResult<CommentResponse>> CreateComment([FromBody] CreateCommentRequest request)
    {
        var result = await _commentServices.CreateCommentAsync(request);
        return Ok(result);
    }

    /// <summary>Deletes one or more comments by ID.</summary>
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteComment([FromQuery] List<string> ids)
    {
        await _commentServices.DeleteCommentAsync(ids);
        return NoContent();
    }
}
