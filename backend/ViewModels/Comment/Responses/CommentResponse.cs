using backend.Entities;

namespace backend.ViewModels.Comment.Responses;
public class CommentResponse
{
    public string ẢticleId { get; set; }
    public string AuthorId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public string Text { get; set; }
    public string? ParentId { get; set; }
}


public static class CommentResponseExtensions
{
    extension(backend.Entities.Comment comment)
    {
        public CommentResponse ToCommentResponse()
        {
            return new CommentResponse
            {
                ẢticleId = comment.ArticleId,
                AuthorId = comment.AuthorId,
                CreatedDate = comment.CreatedDate,
                Text = comment.Text,
                ParentId = comment.ParentId
            };
        }
    }
}
