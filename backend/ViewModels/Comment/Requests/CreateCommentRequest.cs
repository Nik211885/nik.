using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Comment.Requests;

/// <summary>Request model for posting a new comment on an article.</summary>
public class CreateCommentRequest
{
    /// <summary>ID of the article being commented on.</summary>
    public string ArticleId { get; set; }

    /// <summary>Plain-text comment body. Maximum 1 000 characters.</summary>
    public string Text { get; set; }

    /// <summary>ID of the parent comment for threaded replies. <see langword="null"/> for top-level comments.</summary>
    public string? ParentId { get; set; }

    /// <summary>Display name for guest commenters. Required when not authenticated.</summary>
    public string? GuestName { get; set; }

    /// <summary>Email address for guest commenters. Required when not authenticated.</summary>
    public string? GuestEmail { get; set; }

    /// <summary>Optional website URL for guest commenters.</summary>
    public string? GuestWebsite { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateCommentRequest"/>.</summary>
public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    /// <inheritdoc/>
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.GuestEmail).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.GuestEmail));
        RuleFor(x => x.GuestName).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.GuestName));
        RuleFor(x => x.GuestWebsite).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.GuestWebsite));
    }
}
