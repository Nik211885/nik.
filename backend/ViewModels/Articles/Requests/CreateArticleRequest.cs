using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Articles.Requests;

public class CreateArticleRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Image { get; set; }
    public List<string> TagIds { get; set; } = [];
    public List<string> CategoryIds { get; set; } = [];
}

public class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
{
    public CreateArticleRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

public static class CreateArticleRequestExtensions
{
    extension(CreateArticleRequest request)
    {
        public Article ToArticle()
        {
            return new Article
            {
                Title = request.Title,
                Description = request.Description,
                Content = request.Content,
                Image = request.Image
            };
        }
    }
}
