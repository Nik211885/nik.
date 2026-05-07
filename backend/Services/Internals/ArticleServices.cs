using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.Articles.Requests;
using backend.ViewModels.Articles.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class ArticleServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;

    public ArticleServices(
        ILogger<ArticleServices> logger,
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpContext = httpContext;
    }

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

        await DecrementCountRefAsync(tagIds, categoryIds);
    }

    public async Task<ArticleResponse?> GetArticleByIdAsync(string id)
    {
        return await ArticleQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ArticleResponse?> GetArticleBySlugAsync(string slug)
    {
        var result = await ArticleQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (result is not null)
        {
            await _dbContext.Articles
                .Where(a => a.Id == result.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(a => a.CountSee, a => a.CountSee + 1));
        }

        return result;
    }

    public async Task<PaginationItem<ArticleResponse>> GetPaginationArticleAsync(
        GetArticlesPaginationRequest request)
    {
        var query = _dbContext.Articles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.CategorySlug))
        {
            query = query.Where(a => a.ArticleCategories
                .Any(ac => ac.Category.Slug == request.CategorySlug));
        }

        if (!string.IsNullOrWhiteSpace(request.TagSlug))
        {
            query = query.Where(a => a.ArticleTags
                .Any(at => at.Tag.Slug == request.TagSlug));
        }

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.ToLowerInvariant();
            query = query.Where(a =>
                a.Title.ToLower().Contains(keyword) ||
                a.Description.ToLower().Contains(keyword));
        }

        return await query
            .OrderByDescending(a => a.CreatedDate)
            .ToArticleResponses()
            .AsNoTracking()
            .PaginationItemAsync(request);
    }

    private IQueryable<ArticleResponse> ArticleQuery()
    {
        return _dbContext.Articles.ToArticleResponses();
    }

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
