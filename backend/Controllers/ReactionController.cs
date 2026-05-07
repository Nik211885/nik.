using backend.Entities;
using backend.Services.Internals;
using backend.ViewModels;
using backend.ViewModels.Reactions.Requests;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>Endpoints for article reactions (Like, Heart) and paginated reaction listing.</summary>
[ApiController]
[Route("api/reactions")]
public class ReactionController : ControllerBase
{
    private readonly ILogger<ReactionController> _logger;
    private readonly ReactionServices _reactionServices;

    /// <summary>Initialises the controller with required dependencies.</summary>
    public ReactionController(ILogger<ReactionController> logger, ReactionServices reactionServices)
    {
        _logger = logger;
        _reactionServices = reactionServices;
    }

    /// <summary>Records a reaction for the authenticated user on the specified article.</summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateReaction([FromBody] CreateReactionRequest model)
    {
        await _reactionServices.CreateReactionAsync(model);
        return Ok();
    }

    /// <summary>Removes the authenticated user's reaction of the given type from the specified article.</summary>
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteReactionForArticleSpecificType([FromQuery] string articleId, [FromQuery] ReactionType reactionType)
    {
        await _reactionServices.DeleteReactionForArticleSpecificTypeAsync(articleId, reactionType);
        return NoContent();
    }

    /// <summary>Returns a paginated list of reactions left by the authenticated user.</summary>
    [HttpGet("pagination")]
    public async Task<IActionResult> GetPaginationReactionItem([AsParameters] PaginationRequest model)
    {
        var response = await _reactionServices.GetPaginationReactionItemAsync(model);
        return Ok(response);
    }
}
