using backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

/// <summary>
/// EF Core database context. All entity configurations are applied from the assembly
/// via <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/>.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    /// <summary>Translation code keys (e.g. <c>site.title</c>).</summary>
    public DbSet<CodeLanguage> CodeLanguages { get; set; }

    /// <summary>Registered UI languages (e.g. <c>vi</c>, <c>en</c>).</summary>
    public DbSet<Language> Languages { get; set; }

    /// <summary>Per-language translation values keyed by code and language.</summary>
    public DbSet<Translate> Translates { get; set; }

    /// <summary>System configuration key-value entries.</summary>
    public DbSet<SysConfig> SysConfigs { get; set; }

    /// <summary>Photo albums (supports self-referential parent/child hierarchy).</summary>
    public DbSet<Album> Albums { get; set; }

    /// <summary>Join table linking albums to files.</summary>
    public DbSet<AlbumFile> AlbumFiles { get; set; }

    /// <summary>Blog/portfolio articles.</summary>
    public DbSet<Article> Articles { get; set; }

    /// <summary>Join table linking articles to categories.</summary>
    public DbSet<ArticleCategory> ArticleCategories { get; set; }

    /// <summary>Join table linking articles to tags.</summary>
    public DbSet<ArticleTag> ArticleTags { get; set; }

    /// <summary>Content categories with slug-based routing.</summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>Article comments (supports self-referential parent/child nesting).</summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>Cloudinary file metadata records.</summary>
    public DbSet<backend.Entities.File> files { get; set; }

    /// <summary>User reactions (Like, Heart) on articles.</summary>
    public DbSet<Reaction> Reactions { get; set; }

    /// <summary>Content tags with slug-based routing.</summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>Application users.</summary>
    public DbSet<User> Users { get; set; }

    /// <summary>Contact form submissions from the public site.</summary>
    public DbSet<Contact> Contacts { get; set; }

    /// <summary>Page-view events recorded by the public frontend on every route change.</summary>
    public DbSet<PageView> PageViews { get; set; }

    /// <summary>Hero carousel slides shown on the homepage.</summary>
    public DbSet<HeroSlide> HeroSlides { get; set; }

    /// <summary>Translated field values for all translatable content entities.</summary>
    public DbSet<ContentTranslation> ContentTranslations { get; set; }

    /// <summary>Short visitor messages displayed on the public wall page.</summary>
    public DbSet<WallMessage> WallMessages { get; set; }

    /// <summary>Work experience entries for the CV/Careers section.</summary>
    public DbSet<WorkExperience> WorkExperiences { get; set; }

    /// <summary>Skill tags grouped by category for the Skills section.</summary>
    public DbSet<Skill> Skills { get; set; }

    /// <summary>Project entries for the CV/Careers section.</summary>
    public DbSet<Project> Projects { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
