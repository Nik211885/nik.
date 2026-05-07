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

    /// <summary>Initialises the service with required dependencies.</summary>
    public TagServices(ILogger<ArticleServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new tag. The name must be unique (case-insensitive).
    /// </summary>
    /// <param name="model">Tag creation payload.</param>
    /// <returns>The created tag response.</returns>
    /// <exception cref="BadRequestException">Thrown when a tag with the same name already exists.</exception>
    public async Task<TagResponse> CreateTagAsync(CreateTagRequest model)
    {
        var tagExitsName = await _dbContext.Tags.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            ?? throw new BadRequestException();
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
        var tagExitsName = await _dbContext.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase));
        if (tagExitsName?.Id != model.Id)
        {
            throw new BadRequestException();
        }
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

    /// <summary>Deletes one or more tags by ID.</summary>
    /// <param name="ids">IDs of tags to delete.</param>
    public async Task DeleteTagAsync(List<string> ids)
    {
        _ = await _dbContext.Tags.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>Returns all tags.</summary>
    public async Task<IReadOnlyCollection<TagResponse>> GetTagAsync()
    {
        var tags = await _dbContext.Tags.Select(x => x.ToTagResponse())
            .ToListAsync();
        return tags;
    }

    /// <summary>Returns a single tag by ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">Tag ID.</param>
    public async Task<TagResponse?> GetTagByIdAsync(string id)
    {
        var tag = await _dbContext.Tags.Where(x => x.Id == id)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        return tag;
    }

    /// <summary>Returns a single tag by slug, or <see langword="null"/> if not found.</summary>
    /// <param name="slug">URL slug of the tag.</param>
    public async Task<TagResponse?> GetTagBySlugAsync(string slug)
    {
        var tag = await _dbContext.Tags.Where(x => x.Slug == slug)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        return tag;
    }
}
