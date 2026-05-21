using FluentValidation;

namespace backend.ViewModels.WallMessages.Requests;

/// <summary>Payload for submitting a new wall message.</summary>
public class CreateWallMessageRequest
{
    /// <summary>Display name of the visitor.</summary>
    public string Name { get; set; }

    /// <summary>Short message body.</summary>
    public string Message { get; set; }

    /// <summary>Optional source or author attribution.</summary>
    public string? Source { get; set; }
}

/// <inheritdoc/>
public class CreateWallMessageRequestValidator : AbstractValidator<CreateWallMessageRequest>
{
    /// <inheritdoc/>
    public CreateWallMessageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Message).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Source).MaximumLength(200).When(x => x.Source != null);
    }
}
