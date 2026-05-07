namespace backend.Services.Extends;

/// <summary>DI registration for cross-cutting extended services (e.g. caching).</summary>
public static class AddExtendServicesExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers <see cref="CachingServices"/> as a singleton.</summary>
        public IServiceCollection AddExtendServices()
        {
            services.AddSingleton<CachingServices>();
            return services;
        }
    }
}
