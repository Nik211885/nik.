using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Reactions.Requests;
using backend.ViewModels.Reactions.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Provides business operations for article reactions (Like and Heart).
/// Enforces the one-reaction-per-type-per-user-per-article constraint.
/// </summary>
public class ReactionServices
{
    private readonly ILogger<ReactionServices> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public ReactionServices(ILogger<ReactionServices> logger, ApplicationDbContext context, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _context = context;
        _httpContext = httpContext;
    }

    /// <summary>
    /// Records a reaction for the authenticated user on the specified article.
    /// A user may not react with the same type twice on the same article.
    /// </summary>
    /// <param name="model">Reaction payload containing the article ID and reaction type.</param>
    /// <exception cref="NotFoundException">Thrown when the article does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the user has already reacted with the same type.</exception>
    public async Task CreateReactionAsync(CreateReactionRequest model)
    {
        var article = await _context.Reactions.FirstOrDefaultAsync(x => x.ArticleId == model.ArticleId)
            ?? throw new NotFoundException();
        var userId = _httpContext.HttpContext!.GetUserId();

        var reactionTypeExits = await _context.Reactions
            .AsNoTracking().AnyAsync(x => x.ArticleId == model.ArticleId && x.CreatedByUserId == userId && x.ReactionType == model.ReactionType);
        if (reactionTypeExits)
        {
            throw new BadRequestException();
        }

        var reaction = model.ToReactionEntity();
        reaction.CreatedDate = DateTimeOffset.UtcNow;
        reaction.CreatedByUserId = _httpContext.HttpContext!.GetUserId();
        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes the authenticated user's reaction of the given type from the specified article.
    /// No-op if the reaction does not exist.
    /// </summary>
    /// <param name="articleId">ID of the article.</param>
    /// <param name="reactionType">Type of reaction to remove.</param>
    public async Task DeleteReactionForArticleSpecificTypeAsync(string articleId, ReactionType reactionType)
    {
        _ = await _context.Reactions.Where(x => x.ArticleId == articleId && x.CreatedByUserId == _httpContext.HttpContext!.GetUserId() && x.ReactionType == reactionType)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns a paginated list of reactions left by the authenticated user, ordered by newest first.
    /// </summary>
    /// <param name="model">Pagination parameters.</param>
    public async Task<PaginationItem<ReactionResponse>> GetPaginationReactionItemAsync(PaginationRequest model)
    {
        var response = await _context.Reactions.Where(x => x.CreatedByUserId == _httpContext.HttpContext!.GetUserId())
            .OrderByDescending(x => x.CreatedDate)
            .ToReactionResponses()
            .PaginationItemAsync(model);
        return response;
    }
}
