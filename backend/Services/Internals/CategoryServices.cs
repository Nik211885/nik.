using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Category.Requests;
using backend.ViewModels.Category.Responses;
using backend.ViewModels.Tag.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

public class CategoryServices
{
    private readonly ILogger<CategoryServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    public CategoryServices(ILogger<CategoryServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
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
    public async Task<IReadOnlyCollection<CategoryResponse>> GetCategoryAsync()
    {
        var categories = await _dbContext.Categories.Select(x => x.ToCategoryResponse())
            .ToListAsync();
        return categories;
    }
     public async Task DeleteCategoryAsync(List<string> ids)
     {
        _ = await _dbContext.Categories.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
     }
     public async Task<CategoryResponse?> GetByIdAsync(string id)
     {
        var result = await _dbContext.Categories.Where(x => x.Id == id)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        return result;
     }
    public async Task<CategoryResponse?> GetBySlugAsync(string slug)
    {
        var result = await _dbContext.Categories.Where(x => x.Slug == slug)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        return result;
    }

}
