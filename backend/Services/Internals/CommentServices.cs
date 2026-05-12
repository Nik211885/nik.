using backend.Data;
using backend.ViewModels.Comment.Requests;
using backend.ViewModels.Comment.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for article comments.</summary>
public class CommentServices
{
    private readonly ILogger<CommentServices> _logger;
    private readonly ApplicationDbContext _context;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CommentServices(ILogger<CommentServices> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>Returns all comments for the specified article, ordered by creation date.</summary>
    /// <param name="articleId">ID of the article whose comments to retrieve.</param>
    /// <returns>A list of <see cref="CommentResponse"/> objects.</returns>
    public async Task<List<CommentResponse>> GetByArticleAsync(string articleId)
    {
        return await _context.Comments
            .Where(c => c.ArticleId == articleId)
            .OrderBy(c => c.CreatedDate)
            .AsNoTracking()
            .Select(c => c.ToCommentResponse())
            .ToListAsync();
    }

    /// <summary>
    /// Creates and persists a new comment.
    /// </summary>
    /// <param name="model">Comment creation payload.</param>
    /// <returns>The created comment response.</returns>
    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest model)
    {
        var comment = model.ToComment();
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment.ToCommentResponse();
    }

    /// <summary>Deletes one or more comments by ID.</summary>
    /// <param name="ids">IDs of comments to delete.</param>
    public async Task DeleteCommentAsync(List<string> ids)
    {
        await _context.Comments.Where(c => ids.Contains(c.Id.ToString())).ExecuteDeleteAsync();
    }
}
