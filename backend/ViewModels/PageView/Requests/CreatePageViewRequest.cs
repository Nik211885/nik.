using FluentValidation;

namespace backend.ViewModels.PageView.Requests;

/// <summary>Request model sent by the frontend when a route change occurs.</summary>
public class CreatePageViewRequest
{
    /// <summary>The frontend path that was visited (e.g. <c>/travel</c>).</summary>
    public string Path { get; set; }

    /// <summary>Optional referrer URL.</summary>
    public string? Referer { get; set; }
}

/// <summary>FluentValidation rules for <see cref="CreatePageViewRequest"/>.</summary>
public class CreatePageViewRequestValidator : AbstractValidator<CreatePageViewRequest>
{
    /// <inheritdoc/>
    public CreatePageViewRequestValidator()
    {
        RuleFor(x => x.Path).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Referer).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Referer));
    }
}
