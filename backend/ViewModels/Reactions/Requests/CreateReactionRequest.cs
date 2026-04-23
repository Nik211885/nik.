using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Reactions.Requests;
public class CreateReactionRequest
{
    public string ArticleId { get; set; }
    public ReactionType ReactionType{ get; set; }
}

public class CreateReactionRequestValidator : AbstractValidator<CreateReactionRequest>
{
    public CreateReactionRequestValidator()
    {
        RuleFor(x => x.ReactionType).IsInEnum().WithMessage(ApplicationMessage.ReactionTypeIsInvalid);
    }
}

public static class CreateReactionRequestExtension
{
    extension(CreateReactionRequest request)
    {
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
