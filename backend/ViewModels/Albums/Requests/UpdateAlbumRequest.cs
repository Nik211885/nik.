using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

/// <summary>Request model for updating an existing album.</summary>
public class UpdateAlbumRequest
{
    /// <summary>ID of the album to update.</summary>
    public string Id { get; set; }

    /// <summary>Updated lowercase unique name.</summary>
    public string Name { get; set; }

    /// <summary>Updated display title.</summary>
    public string Title { get; set; }

    /// <summary>Updated long-form description.</summary>
    public string Description { get; set; }

    /// <summary>Updated cover file ID.</summary>
    public string FileDescriptionId { get; set; }

    /// <summary>Updated display order index. Must be &gt;= 0.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Updated parent album ID. Pass <see langword="null"/> to move to root.</summary>
    public string? AlbumId { get; set; }
}

/// <summary>FluentValidation rules for <see cref="UpdateAlbumRequest"/>.</summary>
public class UpdateAlbumRequestValidator : AbstractValidator<UpdateAlbumRequest>
{
    /// <inheritdoc/>
    public UpdateAlbumRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage(ApplicationMessage.NameIsRequired);
        RuleFor(x => x.Title).NotEmpty().WithMessage(ApplicationMessage.TitleIsRequired);
        RuleFor(x => x.Description).NotEmpty().WithMessage(ApplicationMessage.DescriptionIsRequired);
        RuleFor(x => x.FileDescriptionId).NotEmpty();
        RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
    }
}

/// <summary>Mapping extensions for <see cref="UpdateAlbumRequest"/>.</summary>
public static class UpdateAlbumRequestExtensions
{
    extension(UpdateAlbumRequest request)
    {
        /// <summary>
        /// Applies mutable fields from the request onto an existing <see cref="Album"/> entity in-place.
        /// Does not change <c>Slug</c> or timestamps — the service handles those.
        /// </summary>
        /// <param name="album">The tracked entity to update.</param>
        /// <returns>The same entity instance with updated properties.</returns>
        public Album ApplyTo(Album album)
        {
            album.Name = request.Name;
            album.Title = request.Title;
            album.Description = request.Description;
            album.FileDescriptionId = request.FileDescriptionId;
            album.OrderIndex = request.OrderIndex;
            album.AlbumId = request.AlbumId;
            return album;
        }
    }
}
