namespace backend.Entities;

/// <summary>
/// Junction entity that models the many-to-many relationship between
/// <see cref="Article"/> and <see cref="Category"/>.
/// Composite primary key: (<see cref="ArticleId"/>, <see cref="CategoryId"/>).
/// </summary>
public class ArticleCategory
{
    /// <summary>Foreign key to the associated <see cref="Article"/>.</summary>
    public string ArticleId { get; set; }

    /// <summary>Foreign key to the associated <see cref="Category"/>.</summary>
    public string CategoryId { get; set; }

    /// <summary>Navigation property for the article.</summary>
    public Article Article { get; set; }

    /// <summary>Navigation property for the category.</summary>
    public Category Category { get; set; }
}
