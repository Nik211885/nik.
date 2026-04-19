using backend.Data;

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
}
