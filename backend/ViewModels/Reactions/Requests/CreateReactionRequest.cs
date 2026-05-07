using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Reactions.Requests;

/// <summary>Request model for adding a reaction to an article.</summary>
public class CreateReactionRequest
{
    /// <summary>ID of the article to react to.</summary>
    public string ArticleId { get; set; }

    /// <summary>Type of reaction being expressed (<see cref="ReactionType.Heart"/> or <see cref="ReactionType.Like"/>).</summary>
    public ReactionType ReactionType { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateReactionRequest"/>.</summary>
public class CreateReactionRequestValidator : AbstractValidator<CreateReactionRequest>
{
    /// <inheritdoc/>
    public CreateReactionRequestValidator()
    {
        RuleFor(x => x.ReactionType).IsInEnum().WithMessage(ApplicationMessage.ReactionTypeIsInvalid);
    }
}

/// <summary>Mapping extensions for <see cref="CreateReactionRequest"/>.</summary>
public static class CreateReactionRequestExtension
{
    extension(CreateReactionRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="Reaction"/> entity.
        /// Does not set <c>CreatedDate</c> or <c>CreatedByUserId</c> — the service sets those.
        /// </summary>
        public Reaction ToReactionEntity()
        {
            return new Reaction
            {
                ArticleId = request.ArticleId,
                ReactionType = request.ReactionType
            };
        }
    }
}
