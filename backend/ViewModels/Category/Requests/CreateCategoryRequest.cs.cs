using FluentValidation;

namespace backend.ViewModels.Category.Requests;

/// <summary>Payload for creating a new category.</summary>
public class CreateCategoryRequest
{
    /// <summary>Unique category name (normalised to lowercase before save).</summary>
    public string Name { get; set; }

    /// <summary>Short description of the category.</summary>
    public string Description { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>URL of the representative category image.</summary>
    public string Image { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateCategoryRequest"/>.</summary>
public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    /// <inheritdoc/>
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="CreateCategoryRequest"/>.</summary>
public static class CreateCategoryRequestExtension
{
    extension(CreateCategoryRequest request)
    {
        /// <summary>Maps the request to a new <see cref="backend.Entities.Category"/> entity.</summary>
        public backend.Entities.Category ToCategoryEntity()
        {
            return new backend.Entities.Category
            {
                Name = request.Name,
                Description = request.Description,
                Title = request.Title,
                Image = request.Image,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
