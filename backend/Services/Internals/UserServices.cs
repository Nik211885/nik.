using backend.Data;

namespace backend.Services.Internals;

public class UserServices
{
    private readonly ILogger<UserServices> _logger;
    private readonly ApplicationDbContext _context;

    public UserServices(ILogger<UserServices> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
}
