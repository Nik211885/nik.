namespace backend.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate{ get; set; }
    public ICollection<ArticleCategory> ArticleCategories { get; set; }
    public int CountRef { get; set; }
}
