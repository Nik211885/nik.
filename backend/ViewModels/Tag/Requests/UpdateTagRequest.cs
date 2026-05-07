using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Tag.Requests;

/// <summary>Request model for updating an existing tag.</summary>
public class UpdateTagRequest
{
    /// <summary>ID of the tag to update.</summary>
    public string Id { get; set; }

    /// <summary>Updated lowercase unique name.</summary>
    public string Name { get; set; }

    /// <summary>Updated display title.</summary>
    public string Title { get; set; }

    /// <summary>Updated description.</summary>
    public string Description { get; set; }

    /// <summary>Updated cover image URL.</summary>
    public string Image { get; set; }
}

/// <summary>FluentValidation rules for <see cref="UpdateTagRequest"/>.</summary>
public abstract class UpdateTagValidator : AbstractValidator<UpdateTagRequest>
{
    /// <inheritdoc/>
    public UpdateTagValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.Image).NotEmpty().WithMessage(ApplicationMessage.ImageIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="UpdateTagRequest"/>.</summary>
public static class UpdateTagExtensions
{
    extension(UpdateTagRequest model)
    {
        /// <summary>
        /// Applies mutable fields from the request onto an existing <see cref="Tag"/> entity in-place.
        /// Does not change <c>Slug</c> or timestamps.
        /// </summary>
        /// <param name="tag">The tracked entity to update.</param>
        /// <returns>The same entity instance with updated properties.</returns>
        public backend.Entities.Tag ToTag(backend.Entities.Tag tag)
        {
            tag.Name = model.Name;
            tag.Title = model.Title;
            tag.Description = model.Description;
            tag.Image = model.Image;
            return tag;
        }
    }
}
