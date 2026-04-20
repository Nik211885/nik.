namespace backend.Entities;

// n-to-n-relationship-between-album-and-file 
public class AlbumFile 
{
    public Album Album { get; set; }
    public File File { get; set; }
    public string AlbumId { get; set; }
    public string FileId { get; set; }
}
