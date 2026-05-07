using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

public class RemoveFilesFromAlbumRequest
{
    public string AlbumId { get; set; }
    public List<string> FileIds { get; set; }
}

public class RemoveFilesFromAlbumRequestValidator : AbstractValidator<RemoveFilesFromAlbumRequest>
{
    public RemoveFilesFromAlbumRequestValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.FileIds).NotEmpty();
        RuleForEach(x => x.FileIds).NotEmpty();
    }
}
