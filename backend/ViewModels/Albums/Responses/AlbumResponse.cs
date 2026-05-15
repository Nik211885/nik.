using backend.Entities;

namespace backend.ViewModels.Albums.Responses;

/// <summary>
/// Album response returned by all album endpoints.
/// When a tree is requested, <see cref="Children"/> is populated recursively.
/// </summary>
public class AlbumResponse
{
    /// <summary>Album ID.</summary>
    public string Id { get; set; }

    /// <summary>Lowercase unique name.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Long-form description.</summary>
    public string Description { get; set; }

    /// <summary>URL-friendly slug.</summary>
    public string Slug { get; set; }

    /// <summary>Denormalised count of files in this album.</summary>
    public int CountImageRef { get; set; }

    /// <summary>Cover file ID. <see langword="null"/> when no cover is set.</summary>
    public string? FileDescriptionId { get; set; }

    /// <summary>Display order index.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Parent album ID. <see langword="null"/> for root albums.</summary>
    public string AlbumId { get; set; }

    /// <summary>Cloudinary URL of the cover file. <see langword="null"/> when no cover is set.</summary>
    public string? CoverUrl { get; set; }

    /// <summary>UTC creation timestamp.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC last-modification timestamp.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>
    /// Direct child albums. Populated only when <c>tree=true</c> is requested;
    /// otherwise <see langword="null"/>.
    /// </summary>
    public List<AlbumResponse> Children { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="Album"/>.</summary>
public static class AlbumResponseExtensions
{
    extension(Album album)
    {
        /// <summary>Maps a single <see cref="Album"/> entity to <see cref="AlbumResponse"/>.</summary>
        public AlbumResponse ToAlbumResponse()
        {
            return new AlbumResponse
            {
                Id = album.Id,
                Name = album.Name,
                Title = album.Title,
                Description = album.Description,
                Slug = album.Slug,
                CountImageRef = album.CountImageRef,
                FileDescriptionId = album.FileDescriptionId,
                CoverUrl = album.File?.Url,
                OrderIndex = album.OrderIndex,
                AlbumId = album.AlbumId,
                CreatedDate = album.CreatedDate,
                UpdatedDate = album.UpdatedDate
            };
        }
    }

    extension(IQueryable<Album> albums)
    {
        /// <summary>
        /// Projects an <see cref="IQueryable{Album}"/> to <see cref="AlbumResponse"/> objects.
        /// Does not populate <see cref="AlbumResponse.Children"/> — use the service tree builder for that.
        /// </summary>
        public IQueryable<AlbumResponse> ToAlbumResponses()
        {
            return albums.Select(album => new AlbumResponse
            {
                Id = album.Id,
                Name = album.Name,
                Title = album.Title,
                Description = album.Description,
                Slug = album.Slug,
                CountImageRef = album.CountImageRef,
                FileDescriptionId = album.FileDescriptionId,
                CoverUrl = album.File != null ? album.File.Url : null,
                OrderIndex = album.OrderIndex,
                AlbumId = album.AlbumId,
                CreatedDate = album.CreatedDate,
                UpdatedDate = album.UpdatedDate
            });
        }
    }
}
