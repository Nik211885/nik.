namespace backend.Entities;

/// <summary>A work experience entry for the CV/Careers section of the about page.</summary>
public class WorkExperience : BaseEntity
{
    /// <summary>Name of the company or organisation.</summary>
    public string Company { get; set; }

    /// <summary>Job title or role held.</summary>
    public string Role { get; set; }

    /// <summary>When the role started.</summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>When the role ended; <see langword="null"/> means currently employed here.</summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>Optional short description of responsibilities or achievements.</summary>
    public string? Description { get; set; }

    /// <summary>Comma-separated technology tags used in this role (e.g. <c>Angular,.NET,Docker</c>).</summary>
    public string? TechTags { get; set; }

    /// <summary>Display order — lower values appear first (most recent).</summary>
    public int Order { get; set; }

    /// <summary>Whether this entry is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}
