using FluentValidation;

namespace backend.ViewModels.Careers.Requests;

/// <summary>Payload for updating an existing work experience entry.</summary>
public class UpdateWorkExperienceRequest
{
    /// <summary>ID of the entry to update.</summary>
    public string Id { get; set; }

    /// <summary>Company or organisation name.</summary>
    public string Company { get; set; }

    /// <summary>Job title or role.</summary>
    public string Role { get; set; }

    /// <summary>Start date of the role.</summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>End date; <see langword="null"/> means currently employed here.</summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>Optional description of responsibilities or achievements.</summary>
    public string? Description { get; set; }

    /// <summary>Comma-separated technology tags.</summary>
    public string? TechTags { get; set; }

    /// <summary>Display order.</summary>
    public int Order { get; set; }

    /// <summary>Whether this entry is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}

/// <inheritdoc/>
public class UpdateWorkExperienceRequestValidator : AbstractValidator<UpdateWorkExperienceRequest>
{
    /// <inheritdoc/>
    public UpdateWorkExperienceRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Company).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Role).NotEmpty().MaximumLength(200);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description != null);
        RuleFor(x => x.TechTags).MaximumLength(500).When(x => x.TechTags != null);
    }
}
