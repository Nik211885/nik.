using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Articles.Requests;
using backend.ViewModels.Articles.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>
/// Provides all business operations for articles, including CRUD, tag/category synchronisation,
/// pagination with filtering, and view-count tracking.
/// </summary>
public class ArticleServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>
    /// Initialises the service with required dependencies.
    /// </summary>
    public ArticleServices(
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
    /// Creates a new article and associates it with the given tags and categories.
    /// Increments <c>CountRef</c> for each tag and category.
    /// </summary>
    /// <param name="request">Article creation payload including content, tags, and categories.</param>
    /// <returns>The created article with full relationship data.</returns>
    public async Task<ArticleResponse> CreateArticleAsync(CreateArticleRequest request)
    {
        var article = request.ToArticle();
        article.Slug = request.Title.ToSlug();
        article.CreatedDate = DateTimeOffset.UtcNow;
        article.UpdatedDate = DateTimeOffset.UtcNow;
        article.AuthorId = _httpContext.HttpContext!.GetUserId();

        _dbContext.Articles.Add(article);

        if (request.TagIds.Count > 0)
        {
            _dbContext.ArticleTags.AddRange(request.TagIds.Select(tagId => new ArticleTag
            {
                ArticleId = article.Id,
                TagId = tagId
            }));
        }

        if (request.CategoryIds.Count > 0)
        {
            _dbContext.ArticleCategories.AddRange(request.CategoryIds.Select(categoryId => new ArticleCategory
            {
                ArticleId = article.Id,
                CategoryId = categoryId
            }));
        }

        await _dbContext.SaveChangesAsync();

        await IncrementCountRefAsync(request.TagIds, request.CategoryIds);

        return await ArticleQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == article.Id)
            ?? throw new NotFoundException();
    }

    /// <summary>
    /// Updates an existing article's content and synchronises its tag/category associations.
    /// Regenerates the slug if the title changes.
    /// </summary>
    /// <param name="request">Update payload. <c>TagIds</c> and <c>CategoryIds</c> are full replacement lists.</param>
    /// <returns>The updated article with full relationship data.</returns>
    /// <exception cref="NotFoundException">Thrown when the article ID does not exist.</exception>
    public async Task<ArticleResponse> UpdateArticleAsync(UpdateArticleRequest request)
    {
        var article = await _dbContext.Articles
            .FirstOrDefaultAsync(a => a.Id == request.Id)
            ?? throw new NotFoundException();

        if (article.Title != request.Title)
            article.Slug = request.Title.ToSlug();

        request.ApplyTo(article);
        article.UpdatedDate = DateTimeOffset.UtcNow;

        await SyncTagsAsync(article.Id, request.TagIds);
        await SyncCategoriesAsync(article.Id, request.CategoryIds);

        _dbContext.Update(article);
        await _dbContext.SaveChangesAsync();

        return await ArticleQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == article.Id)
            ?? throw new NotFoundException();
    }

    /// <summary>
    /// Deletes one or more articles by ID and decrements <c>CountRef</c>
    /// for all their former tags and categories.
    /// </summary>
    /// <param name="ids">IDs of articles to delete.</param>
    public async Task DeleteArticleAsync(List<string> ids)
    {
        var tagIds = await _dbContext.ArticleTags
            .AsNoTracking()
            .Where(at => ids.Contains(at.ArticleId))
            .Select(at => at.TagId)
            .ToListAsync();

        var categoryIds = await _dbContext.ArticleCategories
            .AsNoTracking()
            .Where(ac => ids.Contains(ac.ArticleId))
            .Select(ac => ac.CategoryId)
            .ToListAsync();

        await _dbContext.Articles
            .Where(a => ids.Contains(a.Id))
            .ExecuteDeleteAsync();

        await _translationService.DeleteByEntityAsync(EntityType.Article, ids);
        await DecrementCountRefAsync(tagIds, categoryIds);
    }

    /// <summary>
    /// Returns a single article by its ID, or <see langword="null"/> if not found.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// Does not increment the view count.
    /// </summary>
    /// <param name="id">Article ID.</param>
    public async Task<ArticleResponse?> GetArticleByIdAsync(string id)
    {
        var result = await ArticleQuery().AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (result is not null)
            await OverlayTranslationsAsync(result);
        return result;
    }

    /// <summary>
    /// Returns a single article by its URL slug and increments <see cref="Entities.Article.CountSee"/>
    /// by one. Returns <see langword="null"/> when the slug does not match any article.
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="slug">URL-friendly slug of the article.</param>
    public async Task<ArticleResponse?> GetArticleBySlugAsync(string slug)
    {
        var result = await ArticleQuery().AsNoTracking().FirstOrDefaultAsync(a => a.Slug == slug);

        if (result is not null)
        {
            await _dbContext.Articles
                .Where(a => a.Id == result.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.CountSee, a => a.CountSee + 1));
            await OverlayTranslationsAsync(result);
        }

        return result;
    }

    /// <summary>
    /// Returns a paginated list of articles, optionally filtered by category slug,
    /// tag slug, or a keyword search on title and description.
    /// Results are ordered by <c>CreatedDate</c> descending (newest first).
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="request">Pagination and filter parameters.</param>
    public async Task<PaginationItem<ArticleResponse>> GetPaginationArticleAsync(
        GetArticlesPaginationRequest request)
    {
        var query = _dbContext.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            query = query.Where(a => a.ArticleCategories
                .Any(ac => ac.Category.Slug == request.CategorySlug ||
                           ac.Category.Name == request.CategorySlug));
        }

        if (!string.IsNullOrWhiteSpace(request.TagSlug))
        {
            query = query.Where(a => a.ArticleTags
                .Any(at => at.Tag.Slug == request.TagSlug ||
                           at.Tag.Name == request.TagSlug));
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.ToLowerInvariant();
            query = query.Where(a =>
                a.Title.ToLower().Contains(keyword) ||
                a.Description.ToLower().Contains(keyword));
        }

        var page = await query
            .OrderByDescending(a => a.CreatedDate)
            .ToArticleResponses()
            .AsNoTracking()
            .PaginationItemAsync(request);

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (!ctx.IsAdmin() && lang != "vi" && page.Data.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Article, page.Data.Select(a => a.Id), lang);
            foreach (var item in page.Data)
                if (batch.TryGetValue(item.Id, out var t)) ApplyTranslations(item, t);

            var tagBatch = await _translationService.GetBatchAsync(
                EntityType.Tag, page.Data.SelectMany(a => a.Tags.Select(x => x.Id)).Distinct(), lang);
            var catBatch = await _translationService.GetBatchAsync(
                EntityType.Category, page.Data.SelectMany(a => a.Categories.Select(x => x.Id)).Distinct(), lang);
            foreach (var item in page.Data)
                ApplyNestedTranslations(item, tagBatch, catBatch);
        }

        return page;
    }

    /// <summary>
    /// Returns the top <paramref name="count"/> articles ranked by total engagement score
    /// (views + likes + hearts + comments).
    /// Translates text fields when the request is unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    /// <param name="count">Maximum number of articles to return. Defaults to 12.</param>
    /// <returns>List of article responses ordered by engagement descending.</returns>
    public async Task<List<ArticleResponse>> GetTopArticlesAsync(int count = 12)
    {
        var list = await _dbContext.Articles
            .AsNoTracking()
            .OrderByDescending(a => a.CountSee + a.CountLikeRef + a.CountHeartRef + a.CountCommentRef)
            .Take(count)
            .ToArticleResponses()
            .ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (!ctx.IsAdmin() && lang != "vi" && list.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Article, list.Select(a => a.Id), lang);
            foreach (var item in list)
                if (batch.TryGetValue(item.Id, out var t)) ApplyTranslations(item, t);

            var tagBatch = await _translationService.GetBatchAsync(
                EntityType.Tag, list.SelectMany(a => a.Tags.Select(x => x.Id)).Distinct(), lang);
            var catBatch = await _translationService.GetBatchAsync(
                EntityType.Category, list.SelectMany(a => a.Categories.Select(x => x.Id)).Distinct(), lang);
            foreach (var item in list)
                ApplyNestedTranslations(item, tagBatch, catBatch);
        }

        return list;
    }

    // ── Private helpers ───────────────────────────────────────────────────

    /// <summary>Returns the base article projection query used by all read methods.</summary>
    private IQueryable<ArticleResponse> ArticleQuery()
    {
        return _dbContext.Articles.ToArticleResponses();
    }

    /// <summary>
    /// Overlays translated field values onto a single article response when the request is
    /// unauthenticated and the language is not <c>vi</c>.
    /// </summary>
    private async Task OverlayTranslationsAsync(ArticleResponse r)
    {
        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (ctx.IsAdmin() || lang == "vi") return;
        var t = await _translationService.GetAsync(EntityType.Article, r.Id, lang);
        ApplyTranslations(r, t);

        var tagBatch = await _translationService.GetBatchAsync(
            EntityType.Tag, r.Tags.Select(x => x.Id), lang);
        var catBatch = await _translationService.GetBatchAsync(
            EntityType.Category, r.Categories.Select(x => x.Id), lang);
        ApplyNestedTranslations(r, tagBatch, catBatch);
    }

    private static void ApplyTranslations(ArticleResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("title", out var v)) r.Title = v;
        if (t.TryGetValue("description", out v)) r.Description = v;
        if (t.TryGetValue("content", out v)) r.Content = v;
    }

    private static void ApplyNestedTranslations(
        ArticleResponse r,
        Dictionary<string, Dictionary<string, string>> tagBatch,
        Dictionary<string, Dictionary<string, string>> catBatch)
    {
        foreach (var tag in r.Tags)
            if (tagBatch.TryGetValue(tag.Id, out var t) && t.TryGetValue("title", out var v))
                tag.Title = v;
        foreach (var cat in r.Categories)
            if (catBatch.TryGetValue(cat.Id, out var t) && t.TryGetValue("title", out var v))
                cat.Title = v;
    }

    /// <summary>
    /// Diffs the current tag associations of an article against a new target list and
    /// applies the minimum set of inserts/deletes. Updates <c>Tag.CountRef</c> accordingly.
    /// </summary>
    private async Task SyncTagsAsync(string articleId, List<string> newTagIds)
    {
        var currentTagIds = await _dbContext.ArticleTags
            .AsNoTracking()
            .Where(at => at.ArticleId == articleId)
            .Select(at => at.TagId)
            .ToListAsync();

        var toRemove = currentTagIds.Except(newTagIds).ToList();
        var toAdd = newTagIds.Except(currentTagIds).ToList();

        if (toRemove.Count > 0)
        {
            await _dbContext.ArticleTags
                .Where(at => at.ArticleId == articleId && toRemove.Contains(at.TagId))
                .ExecuteDeleteAsync();

            await _dbContext.Tags
                .Where(t => toRemove.Contains(t.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.CountRef, t => t.CountRef - 1));
        }

        if (toAdd.Count > 0)
        {
            _dbContext.ArticleTags.AddRange(toAdd.Select(tagId => new ArticleTag
            {
                ArticleId = articleId,
                TagId = tagId
            }));

            await _dbContext.Tags
                .Where(t => toAdd.Contains(t.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.CountRef, t => t.CountRef + 1));
        }
    }

    /// <summary>
    /// Diffs the current category associations of an article against a new target list and
    /// applies the minimum set of inserts/deletes. Updates <c>Category.CountRef</c> accordingly.
    /// </summary>
    private async Task SyncCategoriesAsync(string articleId, List<string> newCategoryIds)
    {
        var currentCategoryIds = await _dbContext.ArticleCategories
            .AsNoTracking()
            .Where(ac => ac.ArticleId == articleId)
            .Select(ac => ac.CategoryId)
            .ToListAsync();

        var toRemove = currentCategoryIds.Except(newCategoryIds).ToList();
        var toAdd = newCategoryIds.Except(currentCategoryIds).ToList();

        if (toRemove.Count > 0)
        {
            await _dbContext.ArticleCategories
                .Where(ac => ac.ArticleId == articleId && toRemove.Contains(ac.CategoryId))
                .ExecuteDeleteAsync();

            await _dbContext.Categories
                .Where(c => toRemove.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.CountRef, c => c.CountRef - 1));
        }

        if (toAdd.Count > 0)
        {
            _dbContext.ArticleCategories.AddRange(toAdd.Select(categoryId => new ArticleCategory
            {
                ArticleId = articleId,
                CategoryId = categoryId
            }));

            await _dbContext.Categories
                .Where(c => toAdd.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.CountRef, c => c.CountRef + 1));
        }
    }

    /// <summary>Increments <c>CountRef</c> on all given tags and categories by 1.</summary>
    private async Task IncrementCountRefAsync(List<string> tagIds, List<string> categoryIds)
    {
        if (tagIds.Count > 0)
        {
            await _dbContext.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.CountRef, t => t.CountRef + 1));
        }

        if (categoryIds.Count > 0)
        {
            await _dbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.CountRef, c => c.CountRef + 1));
        }
    }

    /// <summary>
    /// Decrements <c>CountRef</c> on all given tags and categories by 1.
    /// Clamps to zero to prevent negative values.
    /// </summary>
    private async Task DecrementCountRefAsync(List<string> tagIds, List<string> categoryIds)
    {
        if (tagIds.Count > 0)
        {
            await _dbContext.Tags
                .Where(t => tagIds.Contains(t.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(t => t.CountRef, t => Math.Max(0, t.CountRef - 1)));
        }

        if (categoryIds.Count > 0)
        {
            await _dbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.CountRef, c => Math.Max(0, c.CountRef - 1)));
        }
    }
}
