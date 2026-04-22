using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Tag.Requests;
using backend.ViewModels.Tag.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class TagServices
{
    private readonly ILogger<ArticleServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public TagServices(ILogger<ArticleServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
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
    public async Task DeleteTagAsync(List<string> ids)
    {
        _ = await _dbContext.Tags.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }
    public async Task<IReadOnlyCollection<TagResponse>> GetTagAsync()
    {
        var tags = await _dbContext.Tags.Select(x => x.ToTagResponse())
            .ToListAsync();
        return tags;
    }
    public async Task<TagResponse?> GetTagByIdAsync(string id)
    {
        var tag = await _dbContext.Tags.Where(x => x.Id == id)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        return tag;  
    }
    public async Task<TagResponse?> GetTagBySlugAsync(string slug)
    {
        var tag = await _dbContext.Tags.Where(x => x.Slug == slug)
            .Select(x => x.ToTagResponse())
            .FirstOrDefaultAsync();
        return tag;
    }
}
