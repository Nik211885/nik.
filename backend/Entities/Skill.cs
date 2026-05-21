namespace backend.Entities;

/// <summary>A skill tag displayed in the Skills section of the about page.</summary>
public class Skill : BaseEntity
{
    /// <summary>Skill name (e.g. <c>Angular</c>, <c>.NET</c>, <c>Docker</c>).</summary>
    public string Name { get; set; }

    /// <summary>Grouping category (e.g. <c>Frontend</c>, <c>Backend</c>, <c>Tools</c>).</summary>
    public string Category { get; set; }

    /// <summary>Display order within the category — lower values appear first.</summary>
    public int Order { get; set; }

    /// <summary>Whether this skill is visible on the public about page.</summary>
    public bool IsPublished { get; set; }
}
