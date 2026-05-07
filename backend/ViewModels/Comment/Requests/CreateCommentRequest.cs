using System.Runtime.CompilerServices;
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

    /// <summary>
    /// ID of the parent comment for threaded replies.
    /// Pass <see langword="null"/> for a top-level comment.
    /// </summary>
    public string? ParentId { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateCommentRequest"/>.</summary>
public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    /// <inheritdoc/>
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }
}

/// <summary>Mapping extensions for <see cref="CreateCommentRequest"/>.</summary>
public static class CreateCommentExtensions
{
    extension(CreateCommentRequest model)
    {
        /// <summary>
        /// Maps the request to a new <see cref="backend.Entities.Comment"/> entity.
        /// Does not set <c>AuthorId</c> or <c>CreatedDate</c> — the service sets those.
        /// </summary>
        public backend.Entities.Comment ToComment()
        {
            return new backend.Entities.Comment
            {
                ArticleId = model.ArticleId,
                Text = model.Text,
                ParentId = model.ParentId
            };
        }
    }
}
