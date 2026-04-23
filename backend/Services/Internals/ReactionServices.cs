using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Reactions.Requests;
using backend.ViewModels.Reactions.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class ReactionServices
{
    private readonly ILogger<ReactionServices> _logger;
    private readonly ApplicationDbContext _context;
    private readonly HttpContext _httpContext;

    public ReactionServices(ILogger<ReactionServices> logger, ApplicationDbContext context, HttpContext httpContext)
    {
        _logger = logger;
        _context = context;
        _httpContext = httpContext;
    }
    public async Task CreateReactionAsync(CreateReactionRequest model)
    {
        var article = await _context.Reactions.FirstOrDefaultAsync(x => x.ArticleId == model.ArticleId)
            ?? throw new NotFoundException();
        var userId = _httpContext.GetUserId();

        var reactionTypeExits = await _context.Reactions
            .AsNoTracking().AnyAsync(x=>x.ArticleId == model.ArticleId && x.CreatedByUserId == userId && x.ReactionType == model.ReactionType);
        if(reactionTypeExits)
        {
            throw new BadRequestException();
        }

        var reaction = model.ToReactionEntity();
        reaction.CreatedDate = DateTimeOffset.UtcNow;
        reaction.CreatedByUserId = userId;
        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteReactionForArticleSpecificTypeAsync(string articleId, ReactionType reactionType)
    {
        _ = await _context.Reactions.Where(x=>x.ArticleId == articleId && x.CreatedByUserId == _httpContext.GetUserId() && x.ReactionType == reactionType)
            .ExecuteDeleteAsync();
    }
    public async Task<PaginationItem<ReactionResponse>> GetPaginationReactionItemAsync(PaginationRequest model)
    {
        var response = await _context.Reactions.Where(x=>x.CreatedByUserId == _httpContext.GetUserId())
            .OrderByDescending(x=>x.CreatedDate)
            .ToReactionResponses()
            .PaginationItemAsync(model);
        return response;
    }
}
