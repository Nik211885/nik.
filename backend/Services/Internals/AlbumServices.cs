using backend.Data;

namespace backend.Services.Internals;

public class AlbumServices
{
    private readonly ILogger<AlbumServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public AlbumServices(ILogger<AlbumServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}
