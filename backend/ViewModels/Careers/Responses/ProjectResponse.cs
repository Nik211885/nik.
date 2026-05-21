namespace backend.ViewModels.Careers.Responses;

/// <summary>Project entry returned to clients.</summary>
public class ProjectResponse
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; set; }

    /// <summary>Project name.</summary>
    public string Name { get; set; }

    /// <summary>Short description of the project.</summary>
    public string? Description { get; set; }

    /// <summary>Technology tags split into a list.</summary>
    public List<string> TechTags { get; set; } = [];

    /// <summary>Optional link to live demo or website.</summary>
    public string? DemoUrl { get; set; }

    /// <summary>Optional link to source code repository.</summary>
    public string? RepoUrl { get; set; }

    /// <summary>Display order.</summary>
    public int Order { get; set; }

    /// <summary>Whether this project is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}
