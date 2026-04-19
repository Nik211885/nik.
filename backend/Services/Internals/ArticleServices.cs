using backend.Data;

namespace backend.Services.Internals;

public class ArticleServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public ArticleServices(ILogger<ArticleServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
