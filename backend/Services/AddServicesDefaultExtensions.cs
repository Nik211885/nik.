using backend.Services.Extends;
using backend.Services.Internals;

namespace backend.Services;

/// <summary>Top-level DI registration that wires up all internal and extended services.</summary>
public static class AddServicesDefaultExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers all application services (internal business services and cross-cutting extensions).</summary>
        public IServiceCollection AddServicesDefault()
        {
            services.AddExtendServices();
            services.AddServicesInternal();
            return services;
        }
    }
}
