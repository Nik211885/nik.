namespace backend.Services.Internals;

/// <summary>DI registration for all internal business-logic services.</summary>
public static class AddServicesInternalExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers every internal service as scoped.</summary>
        public IServiceCollection AddServicesInternal()
        {
            services.AddScoped<AuthServices>();
            services.AddScoped<AlbumServices>();
            services.AddScoped<ArticleServices>();
            services.AddScoped<CategoryServices>();
            services.AddScoped<CommentServices>();
            services.AddScoped<FileServices>();
            services.AddScoped<ReactionServices>();
            services.AddScoped<TagServices>();
            services.AddScoped<UserServices>();
            services.AddScoped<SysConfigServices>();
            services.AddScoped<LanguageServices>();
            services.AddScoped<ContactServices>();
            services.AddScoped<PageViewServices>();
            services.AddScoped<HeroSlideServices>();
            return services;
        }
    }
}
