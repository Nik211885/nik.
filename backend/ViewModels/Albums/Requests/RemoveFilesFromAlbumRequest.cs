using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

/// <summary>Request model for removing one or more files from an album.</summary>
public class RemoveFilesFromAlbumRequest
{
    /// <summary>ID of the target album.</summary>
    public string AlbumId { get; set; }

    /// <summary>IDs of the files to remove from the album.</summary>
    public List<string> FileIds { get; set; }
}

/// <summary>FluentValidation rules for <see cref="RemoveFilesFromAlbumRequest"/>.</summary>
public class RemoveFilesFromAlbumRequestValidator : AbstractValidator<RemoveFilesFromAlbumRequest>
{
    /// <inheritdoc/>
    public RemoveFilesFromAlbumRequestValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.FileIds).NotEmpty();
        RuleForEach(x => x.FileIds).NotEmpty();
    }
}
