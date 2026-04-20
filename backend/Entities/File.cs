namespace backend.Entities;

public class File : BaseEntity
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Url{ get; set; }
    public string Description { get; set; }
    public ICollection<AlbumFile> AlbumFiles { get; set; }
}
