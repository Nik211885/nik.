namespace backend.ViewModels.Articles.Requests;

/// <summary>
/// Paginated query request for the article list endpoint.
/// All filter fields are optional; omitting them returns all articles ordered by newest first.
/// </summary>
public class GetArticlesPaginationRequest : PaginationRequest
{
    /// <summary>
    /// Filter by category slug. When provided, only articles belonging to the
    /// matching category are returned.
    /// </summary>
    public string? CategorySlug { get; set; }

    /// <summary>
    /// Filter by tag slug. When provided, only articles tagged with the
    /// matching tag are returned.
    /// </summary>
    public string? TagSlug { get; set; }

    /// <summary>
    /// Full-text keyword search applied to <c>Title</c> and <c>Description</c>
    /// (case-insensitive contains match).
    /// </summary>
    public string? Keyword { get; set; }
}
