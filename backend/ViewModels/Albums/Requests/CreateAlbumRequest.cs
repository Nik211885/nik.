using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

/// <summary>Request model for creating a new photo album.</summary>
public class CreateAlbumRequest
{
    /// <summary>Unique lowercase name used for deduplication and slug generation.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Long-form description of the album's content or theme.</summary>
    public string Description { get; set; }

    /// <summary>ID of the <see cref="File"/> used as the album's cover image.</summary>
    public string FileDescriptionId { get; set; }

    /// <summary>Display order relative to sibling albums. Must be &gt;= 0.</summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// ID of the parent album. Pass <see langword="null"/> to create a root-level album.
    /// </summary>
    public string? AlbumId { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreateAlbumRequest"/>.</summary>
public class CreateAlbumRequestValidator : AbstractValidator<CreateAlbumRequest>
{
    /// <inheritdoc/>
    public CreateAlbumRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.FileDescriptionId).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}

/// <summary>Mapping extensions for <see cref="CreateAlbumRequest"/>.</summary>
public static class CreateAlbumRequestExtensions
{
    extension(CreateAlbumRequest request)
    {
        /// <summary>
        /// Maps the request to a new <see cref="Album"/> entity.
        /// Does not set <c>Slug</c>, <c>CreatedDate</c>, <c>UpdatedDate</c>,
        /// or <c>CountImageRef</c> — the service sets those.
        /// </summary>
        public Album ToAlbum()
        {
            return new Album
            {
                Name = request.Name,
                Title = request.Title,
                Description = request.Description,
                FileDescriptionId = request.FileDescriptionId,
                OrderIndex = request.OrderIndex,
                AlbumId = request.AlbumId
            };
        }
    }
}
