using backend.Entities;

namespace backend.ViewModels.Albums.Responses;

/// <summary>
/// Represents a file that belongs to an album, including the file metadata.
/// Returned by the <c>GET /api/albums/{albumId}/files</c> and
/// <c>POST /api/albums/files/add</c> endpoints.
/// </summary>
public class AlbumFileResponse
{
    /// <summary>ID of the file record.</summary>
    public string Id { get; set; }

    /// <summary>ID of the album this file belongs to.</summary>
    public string AlbumId { get; set; }

    /// <summary>Internal lowercase name of the file.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title of the file.</summary>
    public string Title { get; set; }

    /// <summary>Public Cloudinary URL of the file.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption or alt-text for the file.</summary>
    public string Description { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="AlbumFile"/>.</summary>
public static class AlbumFileResponseExtensions
{
    extension(IQueryable<AlbumFile> albumFiles)
    {
        /// <summary>
        /// Projects an <see cref="IQueryable{AlbumFile}"/> to <see cref="AlbumFileResponse"/> objects.
        /// Traverses the <c>File</c> navigation property — ensure EF Core can translate the join.
        /// </summary>
        public IQueryable<AlbumFileResponse> ToAlbumFileResponses()
        {
            return albumFiles.Select(af => new AlbumFileResponse
            {
                Id = af.FileId,
                AlbumId = af.AlbumId,
                Name = af.File.Name,
                Title = af.File.Title,
                Url = af.File.Url,
                Description = af.File.Description
            });
        }
    }
}
