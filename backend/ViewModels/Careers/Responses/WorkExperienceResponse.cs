namespace backend.ViewModels.Careers.Responses;

/// <summary>Work experience entry returned to clients.</summary>
public class WorkExperienceResponse
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; set; }

    /// <summary>Company or organisation name.</summary>
    public string Company { get; set; }

    /// <summary>Job title or role.</summary>
    public string Role { get; set; }

    /// <summary>Start date of the role.</summary>
    public DateTimeOffset StartDate { get; set; }

    /// <summary>End date; <see langword="null"/> means currently employed here.</summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Technology tags split into a list.</summary>
    public List<string> TechTags { get; set; } = [];

    /// <summary>Display order.</summary>
    public int Order { get; set; }

    /// <summary>Whether this entry is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}
