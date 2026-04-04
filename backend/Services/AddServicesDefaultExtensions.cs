namespace backend.Services;

public static class AddServicesDefaultExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServicesDefault()
        {
            services.AddScoped<LanguageServices>();
            return services;
        }
    }
}
