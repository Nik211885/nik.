namespace backend.Entities;

public class Reaction : BaseEntity
{
    public User CreatedByUser { get; set; }
    public Article Article  { get; set; }
    public ReactionType ReactionType { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string CreatedByUserId { get; set; }
    public string ArticleId { get; set; }
}

public enum ReactionType
{
    Heart,
    Like
}
