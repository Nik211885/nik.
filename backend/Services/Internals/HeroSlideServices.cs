using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.HeroSlides.Requests;
using backend.ViewModels.HeroSlides.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides CRUD operations and public retrieval for hero carousel slides.</summary>
public class HeroSlideServices
{
    private readonly ILogger<HeroSlideServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ContentTranslationService _translationService;

    /// <summary>Initialises the service with required dependencies.</summary>
    public HeroSlideServices(
        ILogger<HeroSlideServices> logger,
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContext,
        ContentTranslationService translationService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _httpContext = httpContext;
        _translationService = translationService;
    }

    /// <summary>Returns all slides ordered by <c>OrderIndex</c> ascending.</summary>
    public async Task<List<HeroSlideResponse>> GetAllAsync()
    {
        return await _dbContext.HeroSlides
            .AsNoTracking()
            .OrderBy(x => x.OrderIndex)
            .Select(x => Map(x))
            .ToListAsync();
    }

    /// <summary>
    /// Returns only active slides ordered by <c>OrderIndex</c> for the public carousel.
    /// Translates text fields when the language is not <c>vi</c>.
    /// </summary>
    public async Task<List<HeroSlideResponse>> GetPublicAsync()
    {
        var slides = await _dbContext.HeroSlides
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.OrderIndex)
            .Select(x => Map(x))
            .ToListAsync();

        var lang = _httpContext.HttpContext!.GetLanguage();
        if (lang != "vi" && slides.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.HeroSlide, slides.Select(s => s.Id), lang);
            foreach (var slide in slides)
                if (batch.TryGetValue(slide.Id, out var t)) ApplyTranslations(slide, t);
        }

        return slides;
    }

    /// <summary>Creates a new hero slide.</summary>
    /// <param name="request">Slide creation payload.</param>
    /// <returns>The created slide response.</returns>
    public async Task<HeroSlideResponse> CreateAsync(CreateHeroSlideRequest request)
    {
        var entity = new Entities.HeroSlide
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ImageUrl = request.ImageUrl.Trim(),
            OrderIndex = request.OrderIndex,
            IsActive = request.IsActive
        };

        _dbContext.HeroSlides.Add(entity);
        await _dbContext.SaveChangesAsync();
        return Map(entity);
    }

    /// <summary>Updates an existing hero slide.</summary>
    /// <param name="request">Update payload containing the slide ID and new values.</param>
    /// <returns>The updated slide response.</returns>
    /// <exception cref="NotFoundException">Thrown when no slide with the given ID exists.</exception>
    public async Task<HeroSlideResponse> UpdateAsync(UpdateHeroSlideRequest request)
    {
        var entity = await _dbContext.HeroSlides.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException(ApplicationMessage.NotFound);

        entity.Title = request.Title.Trim();
        entity.Description = request.Description.Trim();
        entity.ImageUrl = request.ImageUrl.Trim();
        entity.OrderIndex = request.OrderIndex;
        entity.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync();
        return Map(entity);
    }

    /// <summary>
    /// Deletes one or more slides by ID and removes their translations.
    /// </summary>
    /// <param name="ids">IDs of slides to delete.</param>
    public async Task DeleteAsync(List<string> ids)
    {
        await _dbContext.HeroSlides.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
        await _translationService.DeleteByEntityAsync(EntityType.HeroSlide, ids);
    }

    // ── Private helpers ───────────────────────────────────────────────────

    private static HeroSlideResponse Map(Entities.HeroSlide x) => new()
    {
        Id = x.Id,
        Title = x.Title,
        Description = x.Description,
        ImageUrl = x.ImageUrl,
        OrderIndex = x.OrderIndex,
        IsActive = x.IsActive
    };

    private static void ApplyTranslations(HeroSlideResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("title", out var v)) r.Title = v;
        if (t.TryGetValue("description", out v)) r.Description = v;
    }
}
