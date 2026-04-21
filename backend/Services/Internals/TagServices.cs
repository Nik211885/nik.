using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Tag.Requests;
using backend.ViewModels.Tag.Responses;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
}
