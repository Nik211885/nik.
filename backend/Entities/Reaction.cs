namespace backend.Entities;

/// <summary>
/// Represents a single reaction (Like or Heart) left by a user on an article.
/// A user can have at most one reaction of each <see cref="ReactionType"/> per article.
/// </summary>
public class Reaction : BaseEntity
{
    /// <summary>Navigation property for the user who reacted.</summary>
    public User CreatedByUser { get; set; }

    /// <summary>Navigation property for the article that was reacted to.</summary>
    public Article Article { get; set; }

    /// <summary>The type of reaction expressed by the user.</summary>
    public ReactionType ReactionType { get; set; }

    /// <summary>UTC timestamp when the reaction was recorded.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Foreign key to the reacting <see cref="User"/>.</summary>
    public string CreatedByUserId { get; set; }

    /// <summary>Foreign key to the target <see cref="Article"/>.</summary>
    public string ArticleId { get; set; }
}

/// <summary>Defines the available reaction types a user can express on an article.</summary>
public enum ReactionType
{
    /// <summary>A heart (love) reaction.</summary>
    Heart,

    /// <summary>A thumbs-up (like) reaction.</summary>
    Like
}
