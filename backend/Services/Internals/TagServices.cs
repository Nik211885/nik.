using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Tag.Requests;
using backend.ViewModels.Tag.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for tags, including CRUD and slug-based lookup.</summary>
public class TagServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the service with required dependencies.</summary>
    public TagServices(
        ILogger<ArticleServices> logger,
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContext,
        ContentTranslationService translationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpContext = httpContext;
        _translationService = translationService;
    }

    /// <summary>
    /// Creates a new tag. The name must be unique (case-insensitive).
    /// </summary>
    /// <param name="model">Tag creation payload.</param>
    /// <returns>The created tag response.</returns>
    /// <exception cref="BadRequestException">Thrown when a tag with the same name already exists.</exception>
    public async Task<TagResponse> CreateTagAsync(CreateTagRequest model)
    {
        var exists = await _dbContext.Tags.AsNoTracking()
            .AnyAsync(x => x.Name.ToLower() == model.Name.ToLower());
        if (exists) throw new BadRequestException(ApplicationMessage.ExitsCode);
        var tag = model.ToTag();
        tag.Slug = tag.Name.ToSlug();
        tag.CreatedDate = DateTimeOffset.UtcNow;
        tag.UpdatedDate = DateTimeOffset.UtcNow;
        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();
        return tag.ToTagResponse();
    }

    /// <summary>
    /// Updates an existing tag. Regenerates the slug when the name changes.
    /// </summary>
    /// <param name="model">Update payload.</param>
    /// <returns>The updated tag response.</returns>
    /// <exception cref="BadRequestException">Thrown when another tag uses the same name, or the tag ID does not exist.</exception>
    public async Task<TagResponse> UpdateTagAsync(UpdateTagRequest model)
    {
        var exists = await _dbContext.Tags.AsNoTracking()
            .AnyAsync(x => x.Name.ToLower() == model.Name.ToLower() && x.Id != model.Id);
        if (exists) throw new BadRequestException(ApplicationMessage.ExitsCode);
        var tagByUpdate = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Id == model.Id)
            ?? throw new BadRequestException();
        if(tagByUpdate.Name != model.Name)
        {
            tagByUpdate.Slug = model.Name.ToSlug();
        }
        model.ToTag(tagByUpdate);
        tagByUpdate.UpdatedDate = DateTimeOffset.UtcNow;
        _dbContext.Update(tagByUpdate);
        await _dbContext.SaveChangesAsync();
        return tagByUpdate.ToTagResponse();
    }

    /// <summary>
    /// Deletes one or more tags by ID and removes their translations.
    /// </summary>
    /// <param name="ids">IDs of tags to delete.</param>
    public async Task DeleteTagAsync(List<string> ids)
    {
        await _dbContext.ArticleTags.Where(at => ids.Contains(at.TagId)).ExecuteDeleteAsync();
        await _dbContext.Tags.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
        await _translationService.DeleteByEntityAsync(EntityType.Tag, ids);
    }

    /// <summary>
    /// Returns all tags.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<TagResponse>> GetTagAsync()
    {
        var tags = await _dbContext.Tags.Select(x => x.ToTagResponse()).ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang != "vi" && tags.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Tag, tags.Select(t => t.Id), lang);
            foreach (var tag in tags)
                if (batch.TryGetValue(tag.Id, out var t)) ApplyTranslations(tag, t);
        }

        return tags;
    }

    /// <summary>
    /// Returns a single tag by ID, or <see langword="null"/> if not found.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="id">Tag ID.</param>
    public async Task<TagResponse?> GetTagByIdAsync(string id)
    {
        var tag = await _dbContext.Tags.Where(x => x.Id == id)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        if (tag is not null) await OverlayTranslationsAsync(tag);
        return tag;
    }

    /// <summary>
    /// Returns a single tag by slug, or <see langword="null"/> if not found.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="slug">URL slug of the tag.</param>
    public async Task<TagResponse?> GetTagBySlugAsync(string slug)
    {
        var tag = await _dbContext.Tags.Where(x => x.Slug == slug)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        if (tag is not null) await OverlayTranslationsAsync(tag);
        return tag;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private async Task OverlayTranslationsAsync(TagResponse r)
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang == "vi") return;
        var t = await _translationService.GetAsync(EntityType.Tag, r.Id, lang);
        ApplyTranslations(r, t);
    }

    private static void ApplyTranslations(TagResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("title", out var v)) r.Title = v;
        if (t.TryGetValue("description", out v)) r.Description = v;
    }
}
