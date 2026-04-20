namespace backend.Entities;

public class Comment : BaseEntity
{
    public string ArticleId { get; set; }
    public string AuthorId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string Text { get; set; }
    public string ParentId { get; set; }
    public User Author { get; set; }
    public Article Article { get; set; }
    public ICollection<Comment> Children { get; set; }
    public Comment Parent { get; set; }

}
