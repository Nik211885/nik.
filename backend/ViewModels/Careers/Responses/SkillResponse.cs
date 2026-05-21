namespace backend.ViewModels.Careers.Responses;

/// <summary>Skill tag returned to clients.</summary>
public class SkillResponse
{
    /// <summary>Unique identifier.</summary>
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
