namespace backend.Services.Extends;
public static class AddExtendServicesExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddExtendServices()
        {
            services.AddSingleton<CachingServices>();
            return services;
        }
    }
}
