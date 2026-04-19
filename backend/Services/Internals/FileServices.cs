using backend.Data;

namespace backend.Services.Internals;

public class FileServices
{
    private readonly ILogger<FileServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public FileServices(ILogger<FileServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
