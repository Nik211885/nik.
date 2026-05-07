using backend.Data;
using backend.Exceptions;
using backend.Extensions;
using backend.ViewModels.Category.Requests;
using backend.ViewModels.Category.Responses;
using backend.ViewModels.Tag.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Internals;

/// <summary>Provides business operations for categories, including CRUD and slug-based lookup.</summary>
public class CategoryServices
{
    private readonly ILogger<CategoryServices> _logger;
    private readonly ApplicationDbContext _dbContext;

    /// <summary>Initialises the service with required dependencies.</summary>
    public CategoryServices(ILogger<CategoryServices> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Creates a new category. The name must be unique (case-insensitive).
    /// </summary>
    /// <param name="model">Category creation payload.</param>
    /// <returns>The created category response.</returns>
    /// <exception cref="BadRequestException">Thrown when a category with the same name already exists.</exception>
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

    /// <summary>
    /// Updates an existing category. Regenerates the slug when the name changes.
    /// </summary>
    /// <param name="model">Update payload.</param>
    /// <returns>The updated category response.</returns>
    /// <exception cref="BadRequestException">Thrown when another category uses the same name, or the category ID does not exist.</exception>
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

    /// <summary>Returns all categories.</summary>
    public async Task<IReadOnlyCollection<CategoryResponse>> GetCategoryAsync()
    {
        var categories = await _dbContext.Categories.Select(x => x.ToCategoryResponse())
            .ToListAsync();
        return categories;
    }

    /// <summary>Deletes one or more categories by ID.</summary>
    /// <param name="ids">IDs of categories to delete.</param>
    public async Task DeleteCategoryAsync(List<string> ids)
    {
        _ = await _dbContext.Categories.Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync();
    }

    /// <summary>Returns a single category by ID, or <see langword="null"/> if not found.</summary>
    /// <param name="id">Category ID.</param>
    public async Task<CategoryResponse?> GetByIdAsync(string id)
    {
        var result = await _dbContext.Categories.Where(x => x.Id == id)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        return result;
    }

    /// <summary>Returns a single category by slug, or <see langword="null"/> if not found.</summary>
    /// <param name="slug">URL slug of the category.</param>
    public async Task<CategoryResponse?> GetBySlugAsync(string slug)
    {
        var result = await _dbContext.Categories.Where(x => x.Slug == slug)
            .Select(x => x.ToCategoryResponse())
            .FirstOrDefaultAsync();
        return result;
    }
}
