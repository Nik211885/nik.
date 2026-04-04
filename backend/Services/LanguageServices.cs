using backend.Data;

namespace backend.Services;

public class LanguageServices
{
    private readonly ILogger<LanguageServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public LanguageServices(ILogger<LanguageServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
