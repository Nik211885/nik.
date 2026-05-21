using FluentValidation;

namespace backend.ViewModels.Careers.Requests;

/// <summary>Payload for updating an existing skill tag.</summary>
public class UpdateSkillRequest
{
    /// <summary>ID of the skill to update.</summary>
    public string Id { get; set; }

    /// <summary>Skill name.</summary>
    public string Name { get; set; }

    /// <summary>Grouping category.</summary>
    public string Category { get; set; }

    /// <summary>Display order within the category.</summary>
    public int Order { get; set; }

    /// <summary>Whether this skill is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}

/// <inheritdoc/>
public class UpdateSkillRequestValidator : AbstractValidator<UpdateSkillRequest>
{
    /// <inheritdoc/>
    public UpdateSkillRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}
