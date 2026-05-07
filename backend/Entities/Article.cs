namespace backend.Entities;

/// <summary>
/// Represents a blog post or portfolio article authored by a user.
/// Tracks engagement counters and maintains many-to-many relationships
/// with <see cref="Tag"/> and <see cref="Category"/>.
/// </summary>
public class Article : BaseEntity
{
    /// <summary>Full HTML or Markdown body of the article.</summary>
    public string Content { get; set; }

    /// <summary>Display title shown in listings and the article page.</summary>
    public string Title { get; set; }

    /// <summary>Short excerpt used in cards and SEO meta descriptions.</summary>
    public string Description { get; set; }

    /// <summary>Cover image URL (hosted on Cloudinary).</summary>
    public string Image { get; set; }

    /// <summary>
    /// URL-friendly identifier in the format <c>{random}/{normalized-title}</c>.
    /// Generated via <see cref="Extensions.StringExtensions.ToSlug"/>.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>UTC timestamp when the article was first saved.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Foreign key to the <see cref="User"/> who wrote the article.</summary>
    public string AuthorId { get; set; }

    /// <summary>Navigation property for the author.</summary>
    public User Author { get; set; }

    /// <summary>Junction records linking this article to its tags.</summary>
    public ICollection<ArticleTag> ArticleTags { get; set; }

    /// <summary>Junction records linking this article to its categories.</summary>
    public ICollection<ArticleCategory> ArticleCategories { get; set; }

    /// <summary>Top-level and nested comments on this article.</summary>
    public ICollection<Comment> Comments { get; set; }

    /// <summary>Denormalised count of comments. Incremented/decremented on comment mutations.</summary>
    public int CountCommentRef { get; set; }

    /// <summary>Denormalised count of <see cref="ReactionType.Like"/> reactions.</summary>
    public int CountLikeRef { get; set; }

    /// <summary>Cumulative view count. Incremented on each slug lookup.</summary>
    public int CountSee { get; set; }

    /// <summary>Denormalised count of <see cref="ReactionType.Heart"/> reactions.</summary>
    public int CountHeartRef { get; set; }
}
