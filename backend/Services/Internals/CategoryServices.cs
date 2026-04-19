using backend.Data;

namespace backend.Services.Internals;

public class CategoryServices
{
    private readonly ILogger<CategoryServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public CategoryServices(ILogger<CategoryServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
