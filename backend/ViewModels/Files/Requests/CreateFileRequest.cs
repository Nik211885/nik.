using FluentValidation;
using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Requests;

/// <summary>
/// Request model for registering a new file record after it has been uploaded to Cloudinary.
/// The binary content is already stored externally; only metadata is saved to the database.
/// </summary>
public class CreateFileRequest
{
    /// <summary>Internal lowercase name used for identification.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title shown in the UI.</summary>
    public string Title { get; set; }

    /// <summary>Public Cloudinary URL of the uploaded file. Must be unique.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption or alt-text description.</summary>
    public string Description { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateFileRequest"/>.</summary>
public class CreateFileRequestValidator : AbstractValidator<CreateFileRequest>
{
    /// <inheritdoc/>
    public CreateFileRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Url).NotEmpty();
    }
}

/// <summary>Mapping extensions for <see cref="CreateFileRequest"/>.</summary>
public static class CreateFileRequestExtensions
{
    extension(CreateFileRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="EFile"/> entity.
        /// </summary>
        public EFile ToFile()
        {
            return new EFile
            {
                Name = request.Name,
                Title = request.Title,
                Url = request.Url,
                Description = request.Description
            };
        }
    }
}
