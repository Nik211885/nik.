using FluentValidation;

namespace backend.ViewModels.Albums.Requests;

/// <summary>Request model for setting or clearing the cover image of an album.</summary>
public class SetCoverRequest
{
    /// <summary>ID of the album to update.</summary>
    public string AlbumId { get; set; }

    /// <summary>ID of the file to use as cover. <see langword="null"/> clears the current cover.</summary>
    public string? FileId { get; set; }
}

/// <summary>FluentValidation rules for <see cref="SetCoverRequest"/>.</summary>
public class SetCoverRequestValidator : AbstractValidator<SetCoverRequest>
{
    /// <inheritdoc/>
    public SetCoverRequestValidator()
    {
        RuleFor(x => x.AlbumId).NotEmpty();
    }
}
