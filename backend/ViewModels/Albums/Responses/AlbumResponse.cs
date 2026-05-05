using backend.Entities;

namespace backend.ViewModels.Albums.Responses;
public class AlbumResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public int CountImageRef { get; set; }
    public string FileDescriptionId { get; set; }
    public int OrderIndex { get; set; }
    public string AlbumId { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset UpdatedDate { get; set; }
    public List<AlbumResponse> Children { get; set; }
}

public static class AlbumResponseExtensions
{
    extension(Album album)
    {
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
                OrderIndex = album.OrderIndex,
                AlbumId = album.AlbumId,
                CreatedDate = album.CreatedDate,
                UpdatedDate = album.UpdatedDate
            };
        }
    }
    extension(IQueryable<Album> albums)
    {
        public IQueryable<AlbumResponse> ToAlbumResponses()
        {
            return albums.Select(album => album.ToAlbumResponse());
        }
    }
}
