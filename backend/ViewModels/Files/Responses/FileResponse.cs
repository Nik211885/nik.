using EFile = backend.Entities.File;

namespace backend.ViewModels.Files.Responses;

public class FileResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

public static class FileResponseExtensions
{
    extension(EFile file)
    {
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
