namespace backend.Entities;

/// <summary>
/// Represents an uploaded file whose binary content is stored on Cloudinary.
/// Only metadata (name, title, URL, description) is persisted in the database.
/// </summary>
public class File : BaseEntity
{
    /// <summary>Internal lowercase name used for identification and deduplication.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title shown in the UI.</summary>
    public string Title { get; set; }

    /// <summary>Public Cloudinary URL of the file. Maximum 2 048 characters.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption or alt-text description of the file content.</summary>
    public string Description { get; set; }

    /// <summary>Junction records linking this file to the albums it belongs to.</summary>
    public ICollection<AlbumFile> AlbumFiles { get; set; }
}
