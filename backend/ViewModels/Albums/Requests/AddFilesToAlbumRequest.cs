using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

/// <summary>Request model for adding one or more files to an album.</summary>
public class AddFilesToAlbumRequest
{
    /// <summary>ID of the target album.</summary>
    public string AlbumId { get; set; }

    /// <summary>IDs of the files to add. Duplicates (already in album) are silently skipped.</summary>
    public List<string> FileIds { get; set; }
}

/// <summary>FluentValidation rules for <see cref="AddFilesToAlbumRequest"/>.</summary>
public class AddFilesToAlbumRequestValidator : AbstractValidator<AddFilesToAlbumRequest>
{
    /// <inheritdoc/>
    public AddFilesToAlbumRequestValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
        RuleFor(x => x.FileIds).NotEmpty();
        RuleForEach(x => x.FileIds).NotEmpty();
    }
}
