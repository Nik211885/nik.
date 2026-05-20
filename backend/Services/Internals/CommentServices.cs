using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Comment.Requests;
using backend.ViewModels.Comment.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for article comments.</summary>
public class CommentServices
{
    private readonly ILogger<CommentServices> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CommentServices(ILogger<CommentServices> logger, ApplicationDbContext context, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _context = context;
        _httpContext = httpContext;
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
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                AuthorId = c.AuthorId,
                AuthorName = c.AuthorId != null ? c.Author.UserName : c.GuestName,
                AuthorAvatar = c.AuthorId != null ? c.Author.Avatar : null,
                GuestWebsite = c.GuestWebsite,
                CreatedDate = c.CreatedDate,
                Text = c.Text,
                ParentId = c.ParentId
            })
            .ToListAsync();
    }

    /// <summary>
    /// Creates and persists a new comment. Authenticated users are identified via JWT;
    /// guests must supply <c>GuestName</c> and <c>GuestEmail</c>.
    /// </summary>
    /// <param name="model">Comment creation payload.</param>
    /// <returns>The created comment response.</returns>
    /// <exception cref="BadRequestException">Thrown when a guest omits required name or email fields.</exception>
    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest model)
    {
        var ctx = _httpContext.HttpContext!;
        var isAuthenticated = ctx.User.Identity?.IsAuthenticated ?? false;
        var hasAuthHeader = ctx.Request.Headers.ContainsKey("Authorization");

        if (!isAuthenticated && hasAuthHeader)
            throw new UnauthorizedException();

        var comment = new Entities.Comment
        {
            ArticleId = model.ArticleId,
            Text = model.Text,
            ParentId = model.ParentId,
            CreatedDate = DateTimeOffset.UtcNow,
        };

        if (isAuthenticated)
        {
            comment.AuthorId = ctx.GetUserId();
        }
        else
        {
            if (string.IsNullOrWhiteSpace(model.GuestName) || string.IsNullOrWhiteSpace(model.GuestEmail))
                throw new BadRequestException(ApplicationMessage.BadRequest);

            comment.GuestName = model.GuestName.Trim();
            comment.GuestEmail = model.GuestEmail.Trim();
            comment.GuestWebsite = model.GuestWebsite?.Trim();
        }

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return await _context.Comments
            .AsNoTracking()
            .Where(c => c.Id == comment.Id)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                AuthorId = c.AuthorId,
                AuthorName = c.AuthorId != null ? c.Author.UserName : c.GuestName,
                AuthorAvatar = c.AuthorId != null ? c.Author.Avatar : null,
                GuestWebsite = c.GuestWebsite,
                CreatedDate = c.CreatedDate,
                Text = c.Text,
                ParentId = c.ParentId
            })
            .FirstAsync();
    }

    /// <summary>Returns a paginated list of comments for admin use, optionally filtered by article.</summary>
    /// <param name="request">Pagination parameters.</param>
    /// <param name="articleId">Optional article ID filter; returns all comments when <see langword="null"/>.</param>
    /// <returns>A paginated result of <see cref="CommentResponse"/>.</returns>
    public async Task<PaginationItem<CommentResponse>> GetPaginationAsync(PaginationRequest request, string? articleId)
    {
        var query = _context.Comments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(articleId))
            query = query.Where(c => c.ArticleId == articleId);

        return await query
            .OrderByDescending(c => c.CreatedDate)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                ArticleId = c.ArticleId,
                AuthorId = c.AuthorId,
                AuthorName = c.AuthorId != null ? c.Author.UserName : c.GuestName,
                AuthorAvatar = c.AuthorId != null ? c.Author.Avatar : null,
                GuestWebsite = c.GuestWebsite,
                CreatedDate = c.CreatedDate,
                Text = c.Text,
                ParentId = c.ParentId
            })
            .PaginationItemAsync(request);
    }

    /// <summary>Deletes one or more comments by ID.</summary>
    /// <param name="ids">IDs of comments to delete.</param>
    public async Task DeleteCommentAsync(List<string> ids)
    {
        await _context.Comments.Where(c => ids.Contains(c.Id)).ExecuteDeleteAsync();
    }
}
