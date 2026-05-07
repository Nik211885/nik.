using backend.Entities;

namespace backend.ViewModels.Category.Responses;

/// <summary>Category response returned by all category endpoints.</summary>
public class CategoryResponse
{
    /// <summary>Category ID.</summary>
    public string Id { get; set; }

    /// <summary>Lowercase unique name.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>URL-friendly slug.</summary>
    public string Slug { get; set; }

    /// <summary>Cover image URL.</summary>
    public string Image { get; set; }

    /// <summary>UTC creation timestamp.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC last-modification timestamp.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Denormalised count of articles in this category.</summary>
    public int CountRef { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="Category"/>.</summary>
public static class CategoryResponseExtension
{
    extension(IQueryable<backend.Entities.Category> query)
    {
        /// <summary>Projects an <see cref="IQueryable{Category}"/> to <see cref="CategoryResponse"/> objects.</summary>
        public IQueryable<CategoryResponse> ToCategoryResponse()
        {
            return query.Select(x => x.ToCategoryResponse());
        }
    }

    extension(backend.Entities.Category entity)
    {
        /// <summary>Maps a single <see cref="Category"/> entity to <see cref="CategoryResponse"/>.</summary>
        public CategoryResponse ToCategoryResponse()
        {
            return new CategoryResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Title = entity.Title,
                Slug = entity.Slug,
                Image = entity.Image,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                CountRef = entity.CountRef
            };
        }
    }
}
