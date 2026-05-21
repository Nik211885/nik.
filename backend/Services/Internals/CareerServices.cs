using backend.Data;
using backend.Entities;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Careers.Requests;
using backend.ViewModels.Careers.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Business logic for work experience and skills (CV/Careers section).</summary>
public class CareerServices
{
    private readonly ILogger<CareerServices> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly ContentTranslationService _translationService;
    private readonly IHttpContextAccessor _httpContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CareerServices(
        ILogger<CareerServices> logger,
        ApplicationDbContext dbContext,
        ContentTranslationService translationService,
        IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _translationService = translationService;
        _httpContext = httpContext;
    }

    // ── Work Experience ───────────────────────────────────────────────────────

    /// <summary>Returns all published work experience entries ordered by display order, with translations applied.</summary>
    public async Task<List<WorkExperienceResponse>> GetPublishedWorkExperiencesAsync()
    {
        var list = await _dbContext.WorkExperiences
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.Order)
            .Select(x => MapWorkExperience(x))
            .ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang != "vi" && list.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.WorkExperience, list.Select(x => x.Id), lang);
            foreach (var item in list)
                if (batch.TryGetValue(item.Id, out var t)) ApplyTranslations(item, t);
        }

        return list;
    }

    /// <summary>Returns all work experience entries for the admin panel (no translation applied).</summary>
    public async Task<List<WorkExperienceResponse>> GetAllWorkExperiencesAsync()
    {
        return await _dbContext.WorkExperiences
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(x => MapWorkExperience(x))
            .ToListAsync();
    }

    /// <summary>Creates a new work experience entry.</summary>
    /// <param name="request">Creation payload.</param>
    /// <returns>The created entry.</returns>
    public async Task<WorkExperienceResponse> CreateWorkExperienceAsync(CreateWorkExperienceRequest request)
    {
        var entity = new WorkExperience
        {
            Company     = request.Company.Trim(),
            Role        = request.Role.Trim(),
            StartDate   = request.StartDate,
            EndDate     = request.EndDate,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            TechTags    = string.IsNullOrWhiteSpace(request.TechTags) ? null : request.TechTags.Trim(),
            Order       = request.Order,
            IsPublished = request.IsPublished,
        };

        _dbContext.WorkExperiences.Add(entity);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("WorkExperience {Id} created: {Role} at {Company}", entity.Id, entity.Role, entity.Company);
        return MapWorkExperience(entity);
    }

    /// <summary>Updates an existing work experience entry.</summary>
    /// <param name="request">Update payload.</param>
    /// <returns>The updated entry.</returns>
    /// <exception cref="NotFoundException">Thrown when no entry with the given ID exists.</exception>
    public async Task<WorkExperienceResponse> UpdateWorkExperienceAsync(UpdateWorkExperienceRequest request)
    {
        var entity = await _dbContext.WorkExperiences.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException();

        entity.Company     = request.Company.Trim();
        entity.Role        = request.Role.Trim();
        entity.StartDate   = request.StartDate;
        entity.EndDate     = request.EndDate;
        entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        entity.TechTags    = string.IsNullOrWhiteSpace(request.TechTags) ? null : request.TechTags.Trim();
        entity.Order       = request.Order;
        entity.IsPublished = request.IsPublished;

        await _dbContext.SaveChangesAsync();
        return MapWorkExperience(entity);
    }

    /// <summary>Deletes a work experience entry and its translations.</summary>
    /// <param name="id">ID of the entry to delete.</param>
    /// <exception cref="NotFoundException">Thrown when no entry with the given ID exists.</exception>
    public async Task DeleteWorkExperienceAsync(string id)
    {
        var deleted = await _dbContext.WorkExperiences
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0) throw new NotFoundException();

        await _translationService.DeleteByEntityAsync(EntityType.WorkExperience, [id]);
    }

    // ── Skills ────────────────────────────────────────────────────────────────

    /// <summary>Returns all published skills ordered by category then display order.</summary>
    public async Task<List<SkillResponse>> GetPublishedSkillsAsync()
    {
        return await _dbContext.Skills
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.Category).ThenBy(x => x.Order)
            .Select(x => MapSkill(x))
            .ToListAsync();
    }

    /// <summary>Returns all skills for the admin panel.</summary>
    public async Task<List<SkillResponse>> GetAllSkillsAsync()
    {
        return await _dbContext.Skills
            .AsNoTracking()
            .OrderBy(x => x.Category).ThenBy(x => x.Order)
            .Select(x => MapSkill(x))
            .ToListAsync();
    }

    /// <summary>Creates a new skill tag.</summary>
    public async Task<SkillResponse> CreateSkillAsync(CreateSkillRequest request)
    {
        var entity = new Skill
        {
            Name        = request.Name.Trim(),
            Category    = request.Category.Trim(),
            Order       = request.Order,
            IsPublished = request.IsPublished,
        };

        _dbContext.Skills.Add(entity);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Skill {Id} created: {Name} in {Category}", entity.Id, entity.Name, entity.Category);
        return MapSkill(entity);
    }

    /// <summary>Updates an existing skill tag.</summary>
    /// <exception cref="NotFoundException">Thrown when no skill with the given ID exists.</exception>
    public async Task<SkillResponse> UpdateSkillAsync(UpdateSkillRequest request)
    {
        var entity = await _dbContext.Skills.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException();

        entity.Name        = request.Name.Trim();
        entity.Category    = request.Category.Trim();
        entity.Order       = request.Order;
        entity.IsPublished = request.IsPublished;

        await _dbContext.SaveChangesAsync();
        return MapSkill(entity);
    }

    /// <summary>Deletes a skill tag by ID.</summary>
    /// <exception cref="NotFoundException">Thrown when no skill with the given ID exists.</exception>
    public async Task DeleteSkillAsync(string id)
    {
        var deleted = await _dbContext.Skills
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0) throw new NotFoundException();
    }

    // ── Projects ──────────────────────────────────────────────────────────────

    /// <summary>Returns all published project entries ordered by display order, with translations applied.</summary>
    public async Task<List<ProjectResponse>> GetPublishedProjectsAsync()
    {
        var list = await _dbContext.Projects
            .AsNoTracking()
            .Where(x => x.IsPublished)
            .OrderBy(x => x.Order)
            .Select(x => MapProject(x))
            .ToListAsync();

        var ctx = _httpContext.HttpContext!;
        var lang = ctx.GetLanguage();
        if (lang != "vi" && list.Count > 0)
        {
            var batch = await _translationService.GetBatchAsync(
                EntityType.Project, list.Select(x => x.Id), lang);
            foreach (var item in list)
                if (batch.TryGetValue(item.Id, out var t)) ApplyProjectTranslations(item, t);
        }

        return list;
    }

    /// <summary>Returns all project entries for the admin panel (no translation applied).</summary>
    public async Task<List<ProjectResponse>> GetAllProjectsAsync()
    {
        return await _dbContext.Projects
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(x => MapProject(x))
            .ToListAsync();
    }

    /// <summary>Creates a new project entry.</summary>
    /// <param name="request">Creation payload.</param>
    /// <returns>The created entry.</returns>
    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
    {
        var entity = new Project
        {
            Name        = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            TechTags    = string.IsNullOrWhiteSpace(request.TechTags) ? null : request.TechTags.Trim(),
            DemoUrl     = string.IsNullOrWhiteSpace(request.DemoUrl) ? null : request.DemoUrl.Trim(),
            RepoUrl     = string.IsNullOrWhiteSpace(request.RepoUrl) ? null : request.RepoUrl.Trim(),
            Order       = request.Order,
            IsPublished = request.IsPublished,
        };

        _dbContext.Projects.Add(entity);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Project {Id} created: {Name}", entity.Id, entity.Name);
        return MapProject(entity);
    }

    /// <summary>Updates an existing project entry.</summary>
    /// <param name="request">Update payload.</param>
    /// <returns>The updated entry.</returns>
    /// <exception cref="NotFoundException">Thrown when no entry with the given ID exists.</exception>
    public async Task<ProjectResponse> UpdateProjectAsync(UpdateProjectRequest request)
    {
        var entity = await _dbContext.Projects.FirstOrDefaultAsync(x => x.Id == request.Id)
            ?? throw new NotFoundException();

        entity.Name        = request.Name.Trim();
        entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        entity.TechTags    = string.IsNullOrWhiteSpace(request.TechTags) ? null : request.TechTags.Trim();
        entity.DemoUrl     = string.IsNullOrWhiteSpace(request.DemoUrl) ? null : request.DemoUrl.Trim();
        entity.RepoUrl     = string.IsNullOrWhiteSpace(request.RepoUrl) ? null : request.RepoUrl.Trim();
        entity.Order       = request.Order;
        entity.IsPublished = request.IsPublished;

        await _dbContext.SaveChangesAsync();
        return MapProject(entity);
    }

    /// <summary>Deletes a project entry and its translations.</summary>
    /// <param name="id">ID of the entry to delete.</param>
    /// <exception cref="NotFoundException">Thrown when no entry with the given ID exists.</exception>
    public async Task DeleteProjectAsync(string id)
    {
        var deleted = await _dbContext.Projects
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        if (deleted == 0) throw new NotFoundException();

        await _translationService.DeleteByEntityAsync(EntityType.Project, [id]);
    }

    // ── Mapping & helpers ─────────────────────────────────────────────────────

    private static WorkExperienceResponse MapWorkExperience(WorkExperience x) => new()
    {
        Id          = x.Id,
        Company     = x.Company,
        Role        = x.Role,
        StartDate   = x.StartDate,
        EndDate     = x.EndDate,
        Description = x.Description,
        TechTags    = string.IsNullOrWhiteSpace(x.TechTags)
                        ? []
                        : [.. x.TechTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
        Order       = x.Order,
        IsPublished = x.IsPublished,
    };

    private static SkillResponse MapSkill(Skill x) => new()
    {
        Id          = x.Id,
        Name        = x.Name,
        Category    = x.Category,
        Order       = x.Order,
        IsPublished = x.IsPublished,
    };

    private static ProjectResponse MapProject(Project x) => new()
    {
        Id          = x.Id,
        Name        = x.Name,
        Description = x.Description,
        TechTags    = string.IsNullOrWhiteSpace(x.TechTags)
                        ? []
                        : [.. x.TechTags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
        DemoUrl     = x.DemoUrl,
        RepoUrl     = x.RepoUrl,
        Order       = x.Order,
        IsPublished = x.IsPublished,
    };

    private static void ApplyTranslations(WorkExperienceResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("role",        out var v)) r.Role        = v;
        if (t.TryGetValue("description", out v))     r.Description = v;
        if (t.TryGetValue("company",     out v))     r.Company     = v;
    }

    private static void ApplyProjectTranslations(ProjectResponse r, Dictionary<string, string> t)
    {
        if (t.TryGetValue("name",        out var v)) r.Name        = v;
        if (t.TryGetValue("description", out v))     r.Description = v;
    }
}
