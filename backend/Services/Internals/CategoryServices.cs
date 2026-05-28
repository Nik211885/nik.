using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Category.Requests;
using backend.ViewModels.Category.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for categories, including CRUD and slug-based lookup.</summary>
public class CategoryServices
{
    private readonly ILogger<CategoryServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CategoryServices(
        ILogger<CategoryServices> logger,
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
    /// Creates a new category. The name must be unique (case-insensitive).
    /// </summary>
    /// <param name="model">Category creation payload.</param>
    /// <returns>The created category response.</returns>
    /// <exception cref="BadRequestException">Thrown when a category with the same name already exists.</exception>
    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest model)
    {
        var categoryExitsName = await _dbContext.Categories.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new BadRequestException();
        var category = model.ToCategoryEntity();
        category.Slug = category.Name.ToSlug();
        category.CreatedDate = DateTimeOffset.UtcNow;
        category.UpdatedDate = DateTimeOffset.UtcNow;
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
        return category.ToCategoryResponse();
    }

    /// <summary>
    /// Updates an existing category. Regenerates the slug when the name changes.
    /// </summary>
    /// <param name="model">Update payload.</param>
    /// <returns>The updated category response.</returns>
    /// <exception cref="BadRequestException">Thrown when another category uses the same name, or the category ID does not exist.</exception>
    public async Task<CategoryResponse> UpdateCategoryAsync(UpdateCategoryRequest model)
    {
        var tagExitsName = await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));
        if (tagExitsName?.Id != model.Id)
        {
            throw new BadRequestException();
        }
        var tagByUpdate = await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == model.Id)
            ?? throw new BadRequestException();
        if (tagByUpdate.Name != model.Name)
        {
            tagByUpdate.Slug = model.Name.ToSlug();
        }
        model.ToCategoryEntity(tagByUpdate);
        tagByUpdate.UpdatedDate = DateTimeOffset.UtcNow;
        _dbContext.Update(tagByUpdate);
        await _dbContext.SaveChangesAsync();
        return tagByUpdate.ToCategoryResponse();
    }

    /// <summary>
    /// Returns all categories.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    public async Task<IReadOnlyCollection<CategoryResponse>> GetCategoryAsync()
    {
        var categories = await _dbContext.Categories.Select(x => x.ToCategoryResponse()).ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang != "vi" && categories.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Category, categories.Select(c => c.Id), lang);
            foreach (var c in categories)
                if (batch.TryGetValue(c.Id, out var t)) ApplyTranslations(c, t);
        }

        return categories;
    }

    /// <summary>
    /// Deletes one or more categories by ID and removes their translations.
    /// </summary>
    /// <param name="ids">IDs of categories to delete.</param>
    public async Task DeleteCategoryAsync(List<string> ids)
    {
        await _dbContext.ArticleCategories.Where(ac => ids.Contains(ac.CategoryId)).ExecuteDeleteAsync();
        await _dbContext.Categories.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
        await _translationService.DeleteByEntityAsync(EntityType.Category, ids);
    }

    /// <summary>
    /// Returns a single category by ID, or <see langword="null"/> if not found.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="id">Category ID.</param>
    public async Task<CategoryResponse?> GetByIdAsync(string id)
    {
        var result = await _dbContext.Categories.Where(x => x.Id == id)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        if (result is not null) await OverlayTranslationsAsync(result);
        return result;
    }

    /// <summary>
    /// Returns a single category by slug, or <see langword="null"/> if not found.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="slug">URL slug of the category.</param>
    public async Task<CategoryResponse?> GetBySlugAsync(string slug)
    {
        var result = await _dbContext.Categories.Where(x => x.Slug == slug)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        if (result is not null) await OverlayTranslationsAsync(result);
        return result;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private async Task OverlayTranslationsAsync(CategoryResponse r)
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang == "vi") return;
        var t = await _translationService.GetAsync(EntityType.Category, r.Id, lang);
        ApplyTranslations(r, t);
    }

    private static void ApplyTranslations(CategoryResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("title", out var v)) r.Title = v;
    }
}
