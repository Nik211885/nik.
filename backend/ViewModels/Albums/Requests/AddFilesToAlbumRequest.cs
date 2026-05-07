using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

public class AddFilesToAlbumRequest
{
    public string AlbumId { get; set; }
    public List<string> FileIds { get; set; }
}

public class AddFilesToAlbumRequestValidator : AbstractValidator<AddFilesToAlbumRequest>
{
    public AddFilesToAlbumRequestValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.FileIds).NotEmpty();
        RuleForEach(x => x.FileIds).NotEmpty();
    }
}
