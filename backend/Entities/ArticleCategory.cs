namespace backend.Entities;
// n-to-n-relationship-between-article-and-category 
public class ArticleCategory 
{
    public string ArticleId { get; set; }
    public string CategoryId { get; set; }
    public Article Article { get; set; }
    public Category Category { get; set; }
}
