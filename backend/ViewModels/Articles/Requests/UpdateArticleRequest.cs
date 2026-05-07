using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Articles.Requests;

public class UpdateArticleRequest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Image { get; set; }
    public List<string> TagIds { get; set; } = [];
    public List<string> CategoryIds { get; set; } = [];
}

public class UpdateArticleRequestValidator : AbstractValidator<UpdateArticleRequest>
{
    public UpdateArticleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

public static class UpdateArticleRequestExtensions
{
    extension(UpdateArticleRequest request)
    {
        public Article ApplyTo(Article article)
        {
            article.Title = request.Title;
            article.Description = request.Description;
            article.Content = request.Content;
            article.Image = request.Image;
            return article;
        }
    }
}
