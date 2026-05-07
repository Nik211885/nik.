using FluentValidation;
using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Requests;

/// <summary>
/// Request model for updating a file's metadata.
/// The Cloudinary URL (<c>Url</c>) is intentionally excluded — it cannot be changed after upload.
/// </summary>
public class UpdateFileRequest
{
    /// <summary>ID of the file to update.</summary>
    public string Id { get; set; }

    /// <summary>Updated internal lowercase name.</summary>
    public string Name { get; set; }

    /// <summary>Updated human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Updated caption or alt-text description.</summary>
    public string Description { get; set; }
}

/// <summary>FluentValidation rules for <see cref="UpdateFileRequest"/>.</summary>
public class UpdateFileRequestValidator : AbstractValidator<UpdateFileRequest>
{
    /// <inheritdoc/>
    public UpdateFileRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="UpdateFileRequest"/>.</summary>
public static class UpdateFileRequestExtensions
{
    extension(UpdateFileRequest request)
    {
        /// <summary>
        /// Applies mutable metadata fields from the request onto an existing <see cref="EFile"/> entity.
        /// Does not change <c>Url</c>.
        /// </summary>
        /// <param name="file">The tracked entity to update in-place.</param>
        /// <returns>The same entity instance with updated properties.</returns>
        public EFile ApplyTo(EFile file)
        {
            file.Name = request.Name;
            file.Title = request.Title;
            file.Description = request.Description;
            return file;
        }
    }
}
