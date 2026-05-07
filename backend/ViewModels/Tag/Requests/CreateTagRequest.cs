using FluentValidation;

namespace backend.ViewModels.Tag.Requests;

/// <summary>Request model for creating a new tag.</summary>
public class CreateTagRequest
{
    /// <summary>Lowercase unique name used for deduplication and slug generation.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Short description of what this tag represents.</summary>
    public string Description { get; set; }

    /// <summary>Cover image URL (hosted on Cloudinary).</summary>
    public string Image { get; set; }
}

/// <summary>Mapping extensions for <see cref="CreateTagRequest"/>.</summary>
public static class CreateTagRequestExtensions
{
    extension(CreateTagRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="backend.Entities.Tag"/> entity.
        /// Does not set <c>Slug</c>, <c>CreatedDate</c>, or <c>UpdatedDate</c>.
        /// </summary>
        public backend.Entities.Tag ToTag()
        {
            return new backend.Entities.Tag
            {
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                Image = request.Image
            };
        }
    }
}

/// <summary>FluentValidation rules for <see cref="CreateTagRequest"/>.</summary>
public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    /// <inheritdoc/>
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}
