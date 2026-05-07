using backend.Entities;

namespace backend.ViewModels.Comment.Responses;

/// <summary>Comment response returned after creation and in listing endpoints.</summary>
public class CommentResponse
{
    /// <summary>ID of the article the comment belongs to.</summary>
    public string ArticleId { get; set; }

    /// <summary>ID of the user who posted the comment.</summary>
    public string AuthorId { get; set; }

    /// <summary>UTC timestamp when the comment was posted.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Plain-text comment body.</summary>
    public string Text { get; set; }

    /// <summary>Parent comment ID for threaded replies. <see langword="null"/> for top-level comments.</summary>
    public string? ParentId { get; set; }
}

/// <summary>Mapping extensions for <see cref="backend.Entities.Comment"/>.</summary>
public static class CommentResponseExtensions
{
    extension(backend.Entities.Comment comment)
    {
        /// <summary>Maps a single <see cref="backend.Entities.Comment"/> entity to <see cref="CommentResponse"/>.</summary>
        public CommentResponse ToCommentResponse()
        {
            return new CommentResponse
            {
                ArticleId = comment.ArticleId,
                AuthorId = comment.AuthorId,
                CreatedDate = comment.CreatedDate,
                Text = comment.Text,
                ParentId = comment.ParentId
            };
        }
    }
}
