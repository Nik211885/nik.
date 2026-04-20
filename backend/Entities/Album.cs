namespace backend.Entities;

public class Album : BaseEntity
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public int CountImageRef { get; set; }
    public File File { get; set; }
    public string FileDescriptionId { get; set; }
    public ICollection<AlbumFile> AlbumFiles { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate{ get; set; }
    public int OrderIndex { get; set; }
    public string AlbumId { get; set; }
    public Album ParentAlbum { get; set; }
    public ICollection<Album> ChildrenAlbum { get; set; }
}
