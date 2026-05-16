using System.Globalization;
using System.Text.Json;
using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.Services;
using backend.ViewModels.Configs.Requests;
using backend.ViewModels.Configs.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides CRUD operations and public aggregation for system configuration entries.</summary>
public class SysConfigServices
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<SysConfigServices> _logger;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the service with required dependencies.</summary>
    public SysConfigServices(
        ApplicationDbContext dbContext,
        ILogger<SysConfigServices> logger,
        IHttpContextAccessor httpContext,
        ContentTranslationService translationService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _httpContext = httpContext;
        _translationService = translationService;
    }

    /// <summary>Returns all configuration entries.</summary>
    public async Task<IReadOnlyCollection<ConfigResponse>> GetConfigsAsync()
    {
        var result = await _dbContext
            .SysConfigs.Select(x => new ConfigResponse()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .AsNoTracking()
            .ToListAsync();
        return result;
    }

    /// <summary>
    /// Creates a new configuration entry. The key is normalised to lowercase and must be unique.
    /// </summary>
    /// <param name="request">Configuration payload.</param>
    /// <returns>The created <see cref="ConfigResponse"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when the key already exists.</exception>
    public async Task<ConfigResponse> CreateConfigAsync(CreateConfigRequest request)
    {
        request.Key = request.Key.ToLowerInvariant();
        var configExitsKey = await _dbContext.SysConfigs
            .Where(x => x.Key == request.Key).AsNoTracking().AnyAsync();
        if (configExitsKey)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        var entity = new SysConfig() { Key = request.Key, Value = request.Value };
        _dbContext.SysConfigs.Add(entity);
        await _dbContext.SaveChangesAsync();

        return new ConfigResponse { Id = entity.Id, Key = entity.Key, Value = entity.Value };
    }

    /// <summary>
    /// Updates an existing configuration entry. The new key must not be used by a different entry.
    /// </summary>
    /// <param name="request">Update payload containing the entry ID, new key and new value.</param>
    /// <returns>The updated <see cref="ConfigResponse"/>.</returns>
    /// <exception cref="NotFoundException">Thrown when the entry ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new key is already used by another entry.</exception>
    public async Task<ConfigResponse> UpdateConfigAsync(UpdateConfigRequest request)
    {
        request.Key = request.Key.ToLowerInvariant();
        var configById = await _dbContext.SysConfigs.Where(x => x.Id == request.Id).FirstOrDefaultAsync()
            ?? throw new NotFoundException(ApplicationMessage.NotFound);

        var configKeyExits = await _dbContext.SysConfigs
            .Where(x => x.Key == request.Key).AsNoTracking().FirstOrDefaultAsync();
        if (configKeyExits is not null && configById.Id != configKeyExits.Id)
        {
            throw new BadRequestException(ApplicationMessage.ExitsCode);
        }

        configById.Key = request.Key;
        configById.Value = request.Value;

        _dbContext.SysConfigs.Update(configById);
        await _dbContext.SaveChangesAsync();

        return new ConfigResponse { Id = configById.Id, Key = configById.Key, Value = configById.Value };
    }

    /// <summary>
    /// Updates the key and value of a specific configuration entry.
    /// The new key must not be used by a different entry.
    /// </summary>
    /// <param name="id">ID of the entry to update.</param>
    /// <param name="request">New key and value.</param>
    /// <exception cref="NotFoundException">Thrown when the entry ID does not exist.</exception>
    /// <exception cref="BadRequestException">Thrown when the new key is already used by another entry.</exception>
    public async Task UpdateConfigSpecificByIdAsync(string id, CreateConfigRequest request)
    {
        var updateRequest = new UpdateConfigRequest { Id = id, Key = request.Key, Value = request.Value };
        await UpdateConfigAsync(updateRequest);
    }

    /// <summary>Returns a single configuration entry by ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">Configuration entry ID.</param>
    public async Task<ConfigResponse?> GetConfigByIdAsync(string id)
    {
        var config = await _dbContext.SysConfigs.Where(x => x.Id == id)
            .Select(x=>new ConfigResponse()
            {
                Id = x.Id,
                Key = x.Key,
                Value = x.Value
            })
            .FirstOrDefaultAsync();
        return config;
    }

    /// <summary>Deletes one or more configuration entries by ID.</summary>
    /// <param name="ids">IDs of entries to delete.</param>
    public async Task DeleteConfigByIdsAsync(List<string> ids)
    {
        await _dbContext.SysConfigs.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns all SysConfig entries keyed by the segment after the first dot
    /// (e.g. <c>config.sidebar</c> → <c>sidebar</c>), plus computed article archive
    /// and category archive statistics.
    /// Multilingual values stored as <c>{ "vi": "...", "en": "..." }</c> are unwrapped
    /// to the requested language; <c>vi</c> is used as fallback when the translation is absent.
    /// </summary>
    public async Task<Dictionary<string, object?>> GetPublicConfigAsync()
    {
        var lang = _httpContext.HttpContext?.GetLanguage() ?? "vi";
        var allConfigs = await _dbContext.SysConfigs.AsNoTracking().ToListAsync();

        var result = new Dictionary<string, object?>();
        foreach (var cfg in allConfigs)
        {
            var dot = cfg.Key.IndexOf('.');
            var key = dot >= 0 ? cfg.Key[(dot + 1)..] : cfg.Key;
            var element = cfg.Value.RootElement;

            if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty(lang, out var langVal))
                result[key] = langVal;
            else if (element.ValueKind == JsonValueKind.Object && lang != "vi" && element.TryGetProperty("vi", out var viVal))
                result[key] = viVal;
            else
                result[key] = element;
        }

        var archivesRaw = await _dbContext.Articles
            .AsNoTracking()
            .GroupBy(a => new { a.CreatedDate.Year, a.CreatedDate.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderByDescending(g => g.Year)
            .ThenByDescending(g => g.Month)
            .ToListAsync();
        result["archivesCountAtTime"] = archivesRaw
            .Select(g => new
            {
                time = lang == "vi"
                    ? $"Tháng {g.Month} năm {g.Year}"
                    : new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy", CultureInfo.InvariantCulture),
                count = g.Count,
                @ref  = $"/archives/{g.Year}/{g.Month:D2}"
            })
            .ToList<object>();

        var categories = await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.CountRef > 0)
            .OrderByDescending(c => c.CountRef)
            .Select(c => new { Id = c.Id, Title = c.Title, Count = c.CountRef, Ref = "/" + c.Slug })
            .ToListAsync();

        if (lang != "vi" && categories.Count > 0)
        {
            var catTrans = await _translationService.GetBatchAsync(
                EntityType.Category, categories.Select(c => c.Id), lang);
            result["categoryCountArchives"] = categories
                .Select(c =>
                {
                    var name = catTrans.TryGetValue(c.Id, out var t) && t.TryGetValue("title", out var v) ? v : c.Title;
                    return (object)new { id = c.Id, name, count = c.Count, @ref = c.Ref };
                })
                .ToList();
        }
        else
        {
            result["categoryCountArchives"] = categories
                .Select(c => (object)new { id = c.Id, name = c.Title, count = c.Count, @ref = c.Ref })
                .ToList();
        }

        return result;
    }
}
