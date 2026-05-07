using backend.Entities;

namespace backend.ViewModels.Albums.Responses;

public class AlbumFileResponse
{
    public string FileId { get; set; }
    public string AlbumId { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

public static class AlbumFileResponseExtensions
{
    extension(IQueryable<AlbumFile> albumFiles)
    {
        public IQueryable<AlbumFileResponse> ToAlbumFileResponses()
        {
            return albumFiles.Select(af => new AlbumFileResponse
            {
                FileId = af.FileId,
                AlbumId = af.AlbumId,
                Name = af.File.Name,
                Title = af.File.Title,
                Url = af.File.Url,
                Description = af.File.Description
            });
        }
    }
}
