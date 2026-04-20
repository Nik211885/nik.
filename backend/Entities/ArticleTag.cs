namespace backend.Entities;
// n-to-n-relationship-between-article-and-tag 
public class ArticleTag 
{
    public string ArticleId { get; set; }
    public string TagId { get; set; }
    public Article Article { get; set; }
    public Tag Tag { get; set; }
}
