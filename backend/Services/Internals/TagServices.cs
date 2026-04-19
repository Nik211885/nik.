using backend.Data;

namespace backend.Services.Internals;

public class TagServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TagServices(ILogger<ArticleServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
