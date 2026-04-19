using backend.Data;

namespace backend.Services.Internals;

public class ReactionServices
{
    private readonly ILogger<ReactionServices> _logger;
    private readonly ApplicationDbContext _context;

    public ReactionServices(ILogger<ReactionServices> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
}
