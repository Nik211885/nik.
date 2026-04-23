using backend.Entities;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.Reactions.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/reactions")]
public class ReactionController : ControllerBase
{
    private readonly ILogger<ReactionController> _logger;
    private readonly ReactionServices _reactionServices;

    public ReactionController(ILogger<ReactionController> logger, ReactionServices reactionServices)
    {
        _logger = logger;
        _reactionServices = reactionServices;
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateReaction([FromBody] CreateReactionRequest model)
    {
        await _reactionServices.CreateReactionAsync(model);
        return Ok();
    }
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteReactionForArticleSpecificType([FromQuery] string articleId, [FromQuery] ReactionType reactionType)
    {
        await _reactionServices.DeleteReactionForArticleSpecificTypeAsync(articleId, reactionType);
        return NoContent();
    }
     [HttpGet("pagination")]
     public async Task<IActionResult> GetPaginationReactionItem([AsParameters] PaginationRequest model)
     {
         var response = await _reactionServices.GetPaginationReactionItemAsync(model);
         return Ok(response);
    }
}
