namespace backend.ViewModels.Articles.Requests;

public class GetArticlesPaginationRequest : PaginationRequest
{
    public string? CategorySlug { get; set; }
    public string? TagSlug { get; set; }
    public string? Keyword { get; set; }
}
