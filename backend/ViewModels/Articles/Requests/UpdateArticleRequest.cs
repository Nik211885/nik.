using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Articles.Requests;

/// <summary>Request model for updating an existing article.</summary>
public class UpdateArticleRequest
{
    /// <summary>ID of the article to update.</summary>
    public string Id { get; set; }

    /// <summary>Updated display title.</summary>
    public string Title { get; set; }

    /// <summary>Updated short excerpt.</summary>
    public string Description { get; set; }

    /// <summary>Updated full HTML or Markdown body.</summary>
    public string Content { get; set; }

    /// <summary>Updated cover image URL.</summary>
    public string Image { get; set; }

    /// <summary>
    /// Complete replacement list of tag IDs.
    /// Tags not in this list will be removed; new ones will be added.
    /// </summary>
    public List<string> TagIds { get; set; } = [];

    /// <summary>
    /// Complete replacement list of category IDs.
    /// Categories not in this list will be removed; new ones will be added.
    /// </summary>
    public List<string> CategoryIds { get; set; } = [];
}

/// <summary>FluentValidation rules for <see cref="UpdateArticleRequest"/>.</summary>
public class UpdateArticleRequestValidator : AbstractValidator<UpdateArticleRequest>
{
    /// <inheritdoc/>
    public UpdateArticleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="UpdateArticleRequest"/>.</summary>
public static class UpdateArticleRequestExtensions
{
    extension(UpdateArticleRequest request)
    {
        /// <summary>
        /// Applies mutable fields from the request onto an existing <see cref="Article"/> entity.
        /// Does not change <c>Slug</c>, <c>AuthorId</c>, or timestamps.
        /// </summary>
        /// <param name="article">The tracked entity to update in-place.</param>
        /// <returns>The same entity instance with updated properties.</returns>
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
