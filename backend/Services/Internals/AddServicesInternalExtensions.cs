namespace backend.Services.Internals;
public static class AddServicesInternalExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServicesInternal()
        {
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
            return services;
        }
    }
}
