namespace backend.Entities;

public class Article : BaseEntity
{
    public string Content { get; set; }
    public string Title { get;set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Slug { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    
    // Foreign key to user
    public string AuthorId { get; set; }
    public User Author { get; set; }
    public ICollection<ArticleTag> ArticleTags { get; set; }
    public ICollection<ArticleCategory> ArticleCategories { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public int CountCommentRef { get; set; }
    public int CountLikeRef { get; set; }
    public int CountSee { get; set; }
    public int CountHeartRef { get; set; }
}
