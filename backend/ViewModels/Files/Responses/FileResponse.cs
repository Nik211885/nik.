using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Responses;

/// <summary>File metadata response returned by all file endpoints.</summary>
public class FileResponse
{
    /// <summary>File ID.</summary>
    public string Id { get; set; }

    /// <summary>Internal lowercase name.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Public Cloudinary URL.</summary>
    public string Url { get; set; }

    /// <summary>Caption or alt-text description.</summary>
    public string Description { get; set; }
}

/// <summary>EF Core-translatable projection extensions for <see cref="EFile"/>.</summary>
public static class FileResponseExtensions
{
    extension(EFile file)
    {
        /// <summary>Maps a single <see cref="EFile"/> entity to <see cref="FileResponse"/>.</summary>
        public FileResponse ToFileResponse()
        {
            return new FileResponse
            {
                Id = file.Id,
                Name = file.Name,
                Title = file.Title,
                Url = file.Url,
                Description = file.Description
            };
        }
    }

    extension(IQueryable<EFile> files)
    {
        /// <summary>
        /// Projects an <see cref="IQueryable{EFile}"/> to <see cref="FileResponse"/> objects.
        /// Fully translatable by EF Core.
        /// </summary>
        public IQueryable<FileResponse> ToFileResponses()
        {
            return files.Select(f => new FileResponse
            {
                Id = f.Id,
                Name = f.Name,
                Title = f.Title,
                Url = f.Url,
                Description = f.Description
            });
        }
    }
}
