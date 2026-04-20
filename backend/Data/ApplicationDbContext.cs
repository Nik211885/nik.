using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<CodeLanguage> CodeLanguages { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Translate> Translates { get; set; }
    public DbSet<SysConfig> SysConfigs { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<AlbumFile> AlbumFiles { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticleCategory> ArticleCategories { get; set; }
    public DbSet<ArticleTag> ArticleTags { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<backend.Entities.File> files { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
