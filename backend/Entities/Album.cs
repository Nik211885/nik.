namespace backend.Entities;

/// <summary>
/// Represents a photo album that can contain files and be nested inside another album.
/// Supports an unlimited hierarchy through the self-referential <see cref="ParentAlbum"/> relationship.
/// </summary>
public class Album : BaseEntity
{
    /// <summary>Lowercase unique name used for deduplication.</summary>
    public string Name { get; set; }

    /// <summary>Human-readable display title.</summary>
    public string Title { get; set; }

    /// <summary>Long-form description of the album's content or theme.</summary>
    public string Description { get; set; }

    /// <summary>
    /// URL-friendly slug in <c>{random}/{normalized-name}</c> format.
    /// Generated via <see cref="Extensions.StringExtensions.ToSlug"/>.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Denormalised count of files in this album.
    /// Updated whenever files are added or removed.
    /// </summary>
    public int CountImageRef { get; set; }

    /// <summary>Navigation property for the cover/description file.</summary>
    public File File { get; set; }

    /// <summary>Foreign key referencing the cover <see cref="File"/>.</summary>
    public string FileDescriptionId { get; set; }

    /// <summary>Junction records linking this album to its files.</summary>
    public ICollection<AlbumFile> AlbumFiles { get; set; }

    /// <summary>UTC timestamp when the album was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Display order relative to sibling albums. Lower values appear first.</summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Foreign key to the parent <see cref="Album"/>. <see langword="null"/> for root albums.
    /// </summary>
    public string? AlbumId { get; set; }

    /// <summary>Navigation property for the parent album.</summary>
    public Album ParentAlbum { get; set; }

    /// <summary>Direct child albums of this album.</summary>
    public ICollection<Album> ChildrenAlbum { get; set; }
}
