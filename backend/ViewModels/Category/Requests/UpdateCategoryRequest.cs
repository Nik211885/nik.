using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Category.Requests;

/// <summary>Payload for updating an existing category.</summary>
public class UpdateCategoryRequest
{
    /// <summary>ID of the category to update.</summary>
    public string Id { get; set; }

    /// <summary>New name (normalised to lowercase before save).</summary>
    public string Name { get; set; }

    /// <summary>Updated short description.</summary>
    public string Description { get; set; }

    /// <summary>Updated display title.</summary>
    public string Title { get; set; }

    /// <summary>Updated representative image URL.</summary>
    public string Image { get; set; }
}

/// <summary>Mapping extensions for <see cref="UpdateCategoryRequest"/>.</summary>
public static class UpdateCategoryRequestExtension
{
    extension(UpdateCategoryRequest request)
    {
        /// <summary>
        /// Applies the request fields to an existing <see cref="backend.Entities.Category"/> entity in-place
        /// and returns it.
        /// </summary>
        public backend.Entities.Category ToCategoryEntity(backend.Entities.Category category)
        {
            category.Name = request.Name;
            category.Description = request.Description;
            category.Title = request.Title;
            category.Image = request.Image;
            category.CreatedDate = DateTimeOffset.UtcNow;
            category.UpdatedDate = DateTimeOffset.UtcNow;
            return category;
        }
    }
}

/// <summary>FluentValidation rules for <see cref="UpdateCategoryRequest"/>.</summary>
public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    /// <inheritdoc/>
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}
