using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels;
using backend.ViewModels.ContentTranslation.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides CRUD operations for the universal content-translation store.</summary>
public class ContentTranslationService
{
    private readonly ILogger<ContentTranslationService> _logger;
    private readonly ApplicationDbContext _context;

    /// <summary>Initialises the service with required dependencies.</summary>
    public ContentTranslationService(ILogger<ContentTranslationService> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Returns the original (Vietnamese) field values for an entity so the translation editor
    /// can display them alongside the translation input fields.
    /// </summary>
    /// <param name="entityType">Logical entity type constant from <see cref="EntityType"/>.</param>
    /// <param name="entityId">ID of the entity.</param>
    /// <returns>Dictionary keyed by field name with the original Vietnamese values.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="entityType"/> is unrecognised.</exception>
    public async Task<Dictionary<string, string>> GetSourceAsync(string entityType, string entityId)
    {
        switch (entityType)
        {
            case EntityType.Article:
            {
                var row = await _context.Articles
                    .Where(a => a.Id == entityId)
                    .Select(a => new { a.Title, a.Description, a.Content })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["title"]       = row.Title,
                    ["description"] = row.Description,
                    ["content"]     = row.Content,
                };
            }
            case EntityType.Category:
            {
                var row = await _context.Categories
                    .Where(c => c.Id == entityId)
                    .Select(c => new { c.Title })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string> { ["title"] = row.Title };
            }
            case EntityType.Tag:
            {
                var row = await _context.Tags
                    .Where(t => t.Id == entityId)
                    .Select(t => new { t.Title, t.Description })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["title"]       = row.Title,
                    ["description"] = row.Description,
                };
            }
            case EntityType.Album:
            {
                var row = await _context.Albums
                    .Where(a => a.Id == entityId)
                    .Select(a => new { a.Title, a.Description })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["title"]       = row.Title,
                    ["description"] = row.Description,
                };
            }
            case EntityType.HeroSlide:
            {
                var row = await _context.HeroSlides
                    .Where(h => h.Id == entityId)
                    .Select(h => new { h.Title, h.Description })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["title"]       = row.Title,
                    ["description"] = row.Description,
                };
            }
            case EntityType.WorkExperience:
            {
                var row = await _context.WorkExperiences
                    .Where(w => w.Id == entityId)
                    .Select(w => new { w.Role, w.Company, w.Description })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["role"]        = row.Role,
                    ["company"]     = row.Company,
                    ["description"] = row.Description ?? string.Empty,
                };
            }
            case EntityType.Project:
            {
                var row = await _context.Projects
                    .Where(p => p.Id == entityId)
                    .Select(p => new { p.Name, p.Description })
                    .FirstOrDefaultAsync();
                return row is null ? [] : new Dictionary<string, string>
                {
                    ["name"]        = row.Name,
                    ["description"] = row.Description ?? string.Empty,
                };
            }
            default:
                throw new BadRequestException(ApplicationMessage.BadRequest);
        }
    }

    /// <summary>
    /// Returns all translated fields for a single entity in the requested language.
    /// Returns an empty dictionary when no translations exist.
    /// </summary>
    /// <param name="entityType">Logical entity type constant from <see cref="EntityType"/>.</param>
    /// <param name="entityId">ID of the entity.</param>
    /// <param name="langCode">BCP-47 base language code (e.g. <c>"en"</c>).</param>
    /// <returns>Dictionary keyed by field name.</returns>
    public async Task<Dictionary<string, string>> GetAsync(
        string entityType, string entityId, string langCode)
    {
        return await _context.ContentTranslations
            .Where(t => t.EntityType == entityType
                     && t.EntityId   == entityId
                     && t.LangCode   == langCode)
            .AsNoTracking()
            .ToDictionaryAsync(t => t.Field, t => t.Value);
    }

    /// <summary>
    /// Returns translated fields for multiple entities in a single query — use this for list
    /// endpoints instead of calling <see cref="GetAsync"/> per item to avoid N+1 queries.
    /// </summary>
    /// <param name="entityType">Logical entity type constant.</param>
    /// <param name="ids">IDs of the entities to load translations for.</param>
    /// <param name="langCode">BCP-47 base language code.</param>
    /// <returns>Dictionary keyed by entity ID, each value is a field→value dictionary.</returns>
    public async Task<Dictionary<string, Dictionary<string, string>>> GetBatchAsync(
        string entityType, IEnumerable<string> ids, string langCode)
    {
        var idList = ids.ToList();
        var rows = await _context.ContentTranslations
            .Where(t => t.EntityType == entityType
                     && idList.Contains(t.EntityId)
                     && t.LangCode   == langCode)
            .AsNoTracking()
            .ToListAsync();

        return rows
            .GroupBy(t => t.EntityId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(t => t.Field, t => t.Value));
    }

    /// <summary>
    /// Creates or updates translated field values for an entity.
    /// Fields with empty or whitespace values are skipped.
    /// </summary>
    /// <param name="entityType">Logical entity type constant.</param>
    /// <param name="entityId">ID of the entity.</param>
    /// <param name="langCode">BCP-47 base language code.</param>
    /// <param name="fields">Field name → translated value pairs to upsert.</param>
    public async Task UpsertAsync(
        string entityType, string entityId, string langCode,
        Dictionary<string, string> fields)
    {
        var existing = await _context.ContentTranslations
            .Where(t => t.EntityType == entityType
                     && t.EntityId   == entityId
                     && t.LangCode   == langCode)
            .ToListAsync();

        foreach (var (field, value) in fields)
        {
            if (string.IsNullOrWhiteSpace(value)) continue;

            var record = existing.FirstOrDefault(t => t.Field == field);
            if (record is null)
                _context.ContentTranslations.Add(new Entities.ContentTranslation
                {
                    EntityType = entityType,
                    EntityId   = entityId,
                    Field      = field,
                    LangCode   = langCode,
                    Value      = value.Trim()
                });
            else
                record.Value = value.Trim();
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes all translations for the specified entities.
    /// Call this inside delete service methods to prevent orphan rows.
    /// </summary>
    /// <param name="entityType">Logical entity type constant.</param>
    /// <param name="entityIds">IDs of the entities whose translations should be removed.</param>
    public async Task DeleteByEntityAsync(string entityType, IEnumerable<string> entityIds)
    {
        var ids = entityIds.ToList();
        await _context.ContentTranslations
            .Where(t => t.EntityType == entityType && ids.Contains(t.EntityId))
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns a paginated list of entities with their translation status for the admin UI.
    /// Uses a LEFT JOIN query — more efficient than correlated subqueries for large datasets.
    /// </summary>
    /// <param name="entityType">Logical entity type constant.</param>
    /// <param name="langCode">Language to check translation status for.</param>
    /// <param name="translated">
    /// <see langword="true"/> = only translated; <see langword="false"/> = only untranslated;
    /// <see langword="null"/> = all.
    /// </param>
    /// <param name="request">Pagination parameters.</param>
    /// <returns>Paginated list of <see cref="TranslationStatusItem"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when <paramref name="entityType"/> is unrecognised.</exception>
    public async Task<PaginationItem<TranslationStatusItem>> GetStatusListAsync(
        string entityType, string langCode, bool? translated, PaginationRequest request)
    {
        IQueryable<TranslationStatusItem> query = entityType switch
        {
            EntityType.Article => (
                from a in _context.Articles.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.Article && ct.LangCode == langCode)
                    on a.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { a.Id, a.Title, a.CreatedDate } into g
                orderby g.Key.CreatedDate descending
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.Article,
                    SourceTitle  = g.Key.Title,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.Category => (
                from c in _context.Categories.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.Category && ct.LangCode == langCode)
                    on c.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { c.Id, c.Title } into g
                orderby g.Key.Title
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.Category,
                    SourceTitle  = g.Key.Title,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.Tag => (
                from tg in _context.Tags.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.Tag && ct.LangCode == langCode)
                    on tg.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { tg.Id, tg.Title } into g
                orderby g.Key.Title
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.Tag,
                    SourceTitle  = g.Key.Title,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.Album => (
                from a in _context.Albums.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.Album && ct.LangCode == langCode)
                    on a.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { a.Id, a.Title } into g
                orderby g.Key.Title
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.Album,
                    SourceTitle  = g.Key.Title,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.HeroSlide => (
                from h in _context.HeroSlides.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.HeroSlide && ct.LangCode == langCode)
                    on h.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { h.Id, h.Title } into g
                orderby g.Key.Title
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.HeroSlide,
                    SourceTitle  = g.Key.Title,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.WorkExperience => (
                from w in _context.WorkExperiences.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.WorkExperience && ct.LangCode == langCode)
                    on w.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { w.Id, w.Role } into g
                orderby g.Key.Role
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.WorkExperience,
                    SourceTitle  = g.Key.Role,
                    IsTranslated = g.Any(t => t != null)
                }),

            EntityType.Project => (
                from p in _context.Projects.AsNoTracking()
                join t in _context.ContentTranslations
                    .Where(ct => ct.EntityType == EntityType.Project && ct.LangCode == langCode)
                    on p.Id equals t.EntityId into ts
                from t in ts.DefaultIfEmpty()
                group t by new { p.Id, p.Name } into g
                orderby g.Key.Name
                select new TranslationStatusItem
                {
                    EntityId     = g.Key.Id,
                    EntityType   = EntityType.Project,
                    SourceTitle  = g.Key.Name,
                    IsTranslated = g.Any(t => t != null)
                }),

            _ => throw new BadRequestException(ApplicationMessage.BadRequest)
        };

        if (translated.HasValue)
            query = query.Where(x => x.IsTranslated == translated.Value);

        return await query.PaginationItemAsync(request);
    }
}
