namespace backend.Entities;

/// <summary>A project entry for the CV/Careers section of the about page.</summary>
public class Project : BaseEntity
{
    /// <summary>Project name.</summary>
    public string Name { get; set; }

    /// <summary>Short description of the project.</summary>
    public string? Description { get; set; }

    /// <summary>Comma-separated technology tags (e.g. <c>Angular,.NET,Docker</c>).</summary>
    public string? TechTags { get; set; }

    /// <summary>Optional link to live demo or website.</summary>
    public string? DemoUrl { get; set; }

    /// <summary>Optional link to source code repository.</summary>
    public string? RepoUrl { get; set; }

    /// <summary>Display order — lower values appear first.</summary>
    public int Order { get; set; }

    /// <summary>Whether this project is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}
