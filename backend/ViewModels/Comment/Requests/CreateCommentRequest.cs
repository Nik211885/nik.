using System.Runtime.CompilerServices;
using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Comment.Requests;
public class CreateCommentRequest
{
    public string ArticleId { get; set; }
    public string Text { get; set; }
    public string? ParentId { get; set; }
}

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(1000);
    }
}

public static class CreateCommentExtensions
{
    extension(CreateCommentRequest model)
    {
        public backend.Entities.Comment ToComment() {
            return new backend.Entities.Comment
            {
                ArticleId = model.ArticleId,
                Text = model.Text,
                ParentId = model.ParentId
            };
        }
    }
}
