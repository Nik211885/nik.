namespace backend.Entities;

/// <summary>
/// Junction entity that models the many-to-many relationship between
/// <see cref="Article"/> and <see cref="Tag"/>.
/// Composite primary key: (<see cref="ArticleId"/>, <see cref="TagId"/>).
/// </summary>
public class ArticleTag
{
    /// <summary>Foreign key to the associated <see cref="Article"/>.</summary>
    public string ArticleId { get; set; }

    /// <summary>Foreign key to the associated <see cref="Tag"/>.</summary>
    public string TagId { get; set; }

    /// <summary>Navigation property for the article.</summary>
    public Article Article { get; set; }

    /// <summary>Navigation property for the tag.</summary>
    public Tag Tag { get; set; }
}
