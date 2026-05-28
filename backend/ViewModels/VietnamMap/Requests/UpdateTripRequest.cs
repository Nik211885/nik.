using FluentValidation;

namespace backend.ViewModels.VietnamMap.Requests;

/// <summary>Payload for updating an existing <see cref="backend.Entities.Trip"/>.</summary>
public class UpdateTripRequest
{
    /// <summary>ID of the trip to update.</summary>
    public string Id { get; set; }

    /// <summary>Updated short title of the trip.</summary>
    public string Title { get; set; }

    /// <summary>Updated date the trip took place.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Updated rich-text HTML story. <see langword="null"/> clears the story.</summary>
    public string? Story { get; set; }
}

/// <summary>Validates <see cref="UpdateTripRequest"/> inputs.</summary>
public class UpdateTripRequestValidator : AbstractValidator<UpdateTripRequest>
{
    /// <inheritdoc/>
    public UpdateTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ApplicationMessage.NotFound);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ApplicationMessage.TitleIsRequired)
            .MaximumLength(255)
            .WithMessage(ApplicationMessage.TitleIsRequired);
    }
}
