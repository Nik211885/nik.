using backend.Pipes.Filter;
using backend.Services;
using backend.Services.Internals;
using backend.ViewModels.Comment.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> _logger;
    private readonly CommentServices _commentServices;

    public CommentController(ILogger<CommentController> logger, CommentServices commentServices)
    {
        _logger = logger;
        _commentServices = commentServices;
    }
    [HttpPost]
    [ValidationFilter(typeof(CreateCommentRequest))]
    public async Task<ActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var result = await _commentServices.CreateCommentAsync(request);
        return Ok(result);
    }
    [HttpDelete]
    public async Task<ActionResult> DeleteComment([FromQuery] List<string> ids)
    {
        await _commentServices.DeleteCommentAsync(ids);
        return NoContent();
    }
}
