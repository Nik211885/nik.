namespace backend.Entities;

/// <summary>
/// Junction entity that models the many-to-many relationship between
/// <see cref="Album"/> and <see cref="File"/>.
/// Composite primary key: (<see cref="AlbumId"/>, <see cref="FileId"/>).
/// </summary>
public class AlbumFile
{
    /// <summary>Foreign key to the owning <see cref="Album"/>.</summary>
    public string AlbumId { get; set; }

    /// <summary>Foreign key to the associated <see cref="File"/>.</summary>
    public string FileId { get; set; }

    /// <summary>Navigation property for the album.</summary>
    public Album Album { get; set; }

    /// <summary>Navigation property for the file.</summary>
    public File File { get; set; }
}
