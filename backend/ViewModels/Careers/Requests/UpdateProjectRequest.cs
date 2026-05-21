using FluentValidation;

namespace backend.ViewModels.Careers.Requests;

/// <summary>Payload for updating an existing project entry.</summary>
public class UpdateProjectRequest
{
    /// <summary>ID of the project to update.</summary>
    public string Id { get; set; }

    /// <summary>Project name.</summary>
    public string Name { get; set; }

    /// <summary>Short description.</summary>
    public string? Description { get; set; }

    /// <summary>Comma-separated technology tags.</summary>
    public string? TechTags { get; set; }

    /// <summary>Optional live demo URL.</summary>
    public string? DemoUrl { get; set; }

    /// <summary>Optional repository URL.</summary>
    public string? RepoUrl { get; set; }

    /// <summary>Display order.</summary>
    public int Order { get; set; }

    /// <summary>Whether visible on the public about page.</summary>
    public bool IsPublished { get; set; } = true;
}

/// <inheritdoc/>
public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
{
    /// <inheritdoc/>
    public UpdateProjectRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(1000).When(x => x.Description != null);
        RuleFor(x => x.TechTags).MaximumLength(500).When(x => x.TechTags != null);
        RuleFor(x => x.DemoUrl).MaximumLength(500).When(x => x.DemoUrl != null);
        RuleFor(x => x.RepoUrl).MaximumLength(500).When(x => x.RepoUrl != null);
    }
}
