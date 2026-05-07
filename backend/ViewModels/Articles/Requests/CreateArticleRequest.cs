using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Articles.Requests;

/// <summary>Request model for creating a new article.</summary>
public class CreateArticleRequest
{
    /// <summary>Display title of the article.</summary>
    public string Title { get; set; }

    /// <summary>Short excerpt used in cards and SEO meta descriptions.</summary>
    public string Description { get; set; }

    /// <summary>Full HTML or Markdown body of the article.</summary>
    public string Content { get; set; }

    /// <summary>Cover image URL (hosted on Cloudinary).</summary>
    public string Image { get; set; }

    /// <summary>IDs of tags to associate with the article. May be empty.</summary>
    public List<string> TagIds { get; set; } = [];

    /// <summary>IDs of categories to associate with the article. May be empty.</summary>
    public List<string> CategoryIds { get; set; } = [];
}

/// <summary>FluentValidation rules for <see cref="CreateArticleRequest"/>.</summary>
public class CreateArticleRequestValidator : AbstractValidator<CreateArticleRequest>
{
    /// <inheritdoc/>
    public CreateArticleRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="CreateArticleRequest"/>.</summary>
public static class CreateArticleRequestExtensions
{
    extension(CreateArticleRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="Article"/> entity.
        /// Does not set <c>Slug</c>, <c>AuthorId</c>, or timestamps — the service sets those.
        /// </summary>
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
