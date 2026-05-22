using FluentValidation;

namespace backend.ViewModels.WallMessages.Requests;

/// <summary>Payload for toggling a reaction on a wall message.</summary>
public class ReactWallMessageRequest
{
    /// <summary>Browser-generated UUID identifying the visitor's device.</summary>
    public string DeviceId { get; set; }
}

/// <inheritdoc/>
public class ReactWallMessageRequestValidator : AbstractValidator<ReactWallMessageRequest>
{
    /// <inheritdoc/>
    public ReactWallMessageRequestValidator()
    {
        RuleFor(x => x.DeviceId).NotEmpty().MaximumLength(36);
    }
}
