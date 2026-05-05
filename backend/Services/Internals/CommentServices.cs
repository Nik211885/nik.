using backend.Data;
using backend.ViewModels.Comment.Requests;
using backend.ViewModels.Comment.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class CommentServices
{
    private readonly ILogger<CommentServices> _logger;
    private readonly ApplicationDbContext _context;

    public CommentServices(ILogger<CommentServices> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<CommentResponse> CreateCommentAsync(CreateCommentRequest model)
    {
        var comment = model.ToComment();
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();
        return comment.ToCommentResponse();
    }
    public async Task DeleteCommentAsync(List<string> ids)
    {
        await _context.Comments.Where(c => ids.Contains(c.Id.ToString())).ExecuteDeleteAsync();
    }
}
