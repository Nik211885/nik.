using FluentValidation;

namespace backend.ViewModels.Careers.Requests;

/// <summary>Payload for creating a new skill tag.</summary>
public class CreateSkillRequest
{
    /// <summary>Skill name.</summary>
    public string Name { get; set; }

    /// <summary>Grouping category (e.g. Frontend, Backend, Tools).</summary>
    public string Category { get; set; }

    /// <summary>Display order within the category.</summary>
    public int Order { get; set; }

    /// <summary>Whether this skill is visible on the public about page.</summary>
    public bool IsPublished { get; set; } = true;
}

/// <inheritdoc/>
public class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>
{
    /// <inheritdoc/>
    public CreateSkillRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
