using backend.Data;

namespace backend.Services.Internals;

/// <summary>Provides business operations for user management.</summary>
public class UserServices
{
    private readonly ILogger<UserServices> _logger;
    private readonly ApplicationDbContext _context;

    /// <summary>Initialises the service with required dependencies.</summary>
    public UserServices(ILogger<UserServices> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
}
