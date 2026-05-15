using FluentValidation;

namespace backend.ViewModels.HeroSlides.Requests;

/// <summary>Request model for creating a new hero slide.</summary>
public class CreateHeroSlideRequest
{
    /// <summary>Slide heading text.</summary>
    public string Title { get; set; }

    /// <summary>Slide sub-text.</summary>
    public string Description { get; set; }

    /// <summary>Background image URL.</summary>
    public string ImageUrl { get; set; }

    /// <summary>Display order index.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this slide is visible on the public site.</summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>FluentValidation rules for <see cref="CreateHeroSlideRequest"/>.</summary>
public class CreateHeroSlideRequestValidator : AbstractValidator<CreateHeroSlideRequest>
{
    /// <inheritdoc/>
    public CreateHeroSlideRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(1000);
    }
}

/// <summary>Request model for updating an existing hero slide.</summary>
public class UpdateHeroSlideRequest
{
    /// <summary>ID of the slide to update.</summary>
    public string Id { get; set; }

    /// <summary>Slide heading text.</summary>
    public string Title { get; set; }

    /// <summary>Slide sub-text.</summary>
    public string Description { get; set; }

    /// <summary>Background image URL.</summary>
    public string ImageUrl { get; set; }

    /// <summary>Display order index.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this slide is visible on the public site.</summary>
    public bool IsActive { get; set; }
}

/// <summary>FluentValidation rules for <see cref="UpdateHeroSlideRequest"/>.</summary>
public class UpdateHeroSlideRequestValidator : AbstractValidator<UpdateHeroSlideRequest>
{
    /// <inheritdoc/>
    public UpdateHeroSlideRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(1000);
    }
}
