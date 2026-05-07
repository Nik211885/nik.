namespace backend.Entities;

/// <summary>
/// Represents a user comment on an article. Supports threaded replies through
/// the self-referential <see cref="Parent"/>/<see cref="Children"/> relationship.
/// </summary>
public class Comment : BaseEntity
{
    /// <summary>Foreign key to the <see cref="Article"/> being commented on.</summary>
    public string ArticleId { get; set; }

    /// <summary>Foreign key to the <see cref="User"/> who wrote the comment.</summary>
    public string AuthorId { get; set; }

    /// <summary>UTC timestamp when the comment was posted.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Plain-text body of the comment. Maximum 1 000 characters.</summary>
    public string Text { get; set; }

    /// <summary>
    /// Foreign key to the parent <see cref="Comment"/> for nested replies.
    /// <see langword="null"/> for top-level comments.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>Navigation property for the comment author.</summary>
    public User Author { get; set; }

    /// <summary>Navigation property for the article this comment belongs to.</summary>
    public Article Article { get; set; }

    /// <summary>Direct child replies to this comment.</summary>
    public ICollection<Comment> Children { get; set; }

    /// <summary>Navigation property for the parent comment, if any.</summary>
    public Comment Parent { get; set; }
}
