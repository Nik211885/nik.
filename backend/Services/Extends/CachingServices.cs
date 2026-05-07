using Microsoft.Extensions.Caching.Memory;

namespace backend.Services.Extends;

/// <summary>
/// Wrapper around <see cref="IMemoryCache"/> for application-level caching.
/// Expose all required configuration at the top via the DI container.
/// When upper layers need this service they must read configuration and pass it as a model.
/// </summary>
public class CachingServices
{
    private readonly IMemoryCache _memoryCaching;
    private readonly ILogger<CachingServices> _logger;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CachingServices(ILogger<CachingServices> logger, IMemoryCache memoryCache)
    {
        _memoryCaching = memoryCache;
        _logger = logger;
    }
}
