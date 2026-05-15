using backend.Entities;

namespace backend.ViewModels.Comment.Responses;

/// <summary>Comment response returned after creation and in listing endpoints.</summary>
public class CommentResponse
{
    /// <summary>Comment ID.</summary>
    public string Id { get; set; }

    /// <summary>ID of the article the comment belongs to.</summary>
    public string ArticleId { get; set; }

    /// <summary>ID of the registered user who posted the comment. <see langword="null"/> for guest comments.</summary>
    public string? AuthorId { get; set; }

    /// <summary>Display name: author's username for registered users, GuestName for guests.</summary>
    public string AuthorName { get; set; }

    /// <summary>Profile picture URL. <see langword="null"/> for guest comments.</summary>
    public string? AuthorAvatar { get; set; }

    /// <summary>Optional website URL provided by guest commenters.</summary>
    public string? GuestWebsite { get; set; }

    /// <summary>UTC timestamp when the comment was posted.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Plain-text comment body.</summary>
    public string Text { get; set; }

    /// <summary>Parent comment ID for threaded replies. <see langword="null"/> for top-level comments.</summary>
    public string? ParentId { get; set; }
}
