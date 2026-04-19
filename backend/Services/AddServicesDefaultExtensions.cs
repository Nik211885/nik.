using backend.Services.Extends;
using backend.Services.Internals;

namespace backend.Services;

public static class AddServicesDefaultExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServicesDefault()
        {
            services.AddExtendServices();
            services.AddServicesInternal();
            return services;
        }
    }
}
