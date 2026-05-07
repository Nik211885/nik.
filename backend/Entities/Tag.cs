namespace backend.Entities;

/// <summary>
/// Represents a keyword tag that can be applied to multiple articles.
/// Tracks how many articles reference it via <see cref="CountRef"/>.
/// </summary>
public class Tag : BaseEntity
{
    /// <summary>Lowercase unique name used for deduplication.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>
    /// URL-friendly slug in <c>{random}/{normalized-name}</c> format.
    /// Generated via <see cref="Extensions.StringExtensions.ToSlug"/>.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>Junction records linking this tag to its articles.</summary>
    public ICollection<ArticleTag> ArticleTags { get; set; }

    /// <summary>UTC timestamp when the tag was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Short description of what this tag represents.</summary>
    public string Description { get; set; }

    /// <summary>Cover image URL (hosted on Cloudinary).</summary>
    public string Image { get; set; }

    /// <summary>
    /// Denormalised count of articles tagged with this tag.
    /// Incremented on article creation/tag-add, decremented on deletion/tag-remove.
    /// </summary>
    public int CountRef { get; set; }
}
