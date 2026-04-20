using Microsoft.Extensions.Caching.Memory;

namespace backend.Services.Extends;

// you must expose all configuration on top with extension di container add
// when top layer use it must read configuration and pass to this service, use model to hold data for service need config
public class CachingServices
{
    private readonly IMemoryCache _memoryCaching;
    private readonly ILogger<CachingServices> _logger;
    public CachingServices(ILogger<CachingServices> logger, IMemoryCache memoryCache)
    {
        _memoryCaching = memoryCache;
        _logger = logger;
    }
}



