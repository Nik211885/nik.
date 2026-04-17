using backend.Data;

namespace backend.Services;

public class SysConfigServices
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SysConfigServices> _logger;

    public SysConfigServices(ApplicationDbContext dbContext, ILogger<SysConfigServices> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
}
