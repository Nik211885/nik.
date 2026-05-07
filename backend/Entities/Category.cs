namespace backend.Entities;

/// <summary>
/// Represents a content category used to group related articles.
/// Tracks how many articles reference it via <see cref="CountRef"/>.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>Lowercase unique name used for deduplication.</summary>
    public string Name { get; set; }

    /// <summary>Long-form description of what this category covers.</summary>
    public string Description { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>
    /// URL-friendly slug in <c>{random}/{normalized-name}</c> format.
    /// Generated via <see cref="Extensions.StringExtensions.ToSlug"/>.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>Cover image URL (hosted on Cloudinary).</summary>
    public string Image { get; set; }

    /// <summary>UTC timestamp when the category was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Junction records linking this category to its articles.</summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; }

    /// <summary>
    /// Denormalised count of articles in this category.
    /// Incremented on article creation/tag-add, decremented on deletion/tag-remove.
    /// </summary>
    public int CountRef { get; set; }
}
