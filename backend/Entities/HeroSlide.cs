namespace backend.Entities;

/// <summary>Represents a single slide in the homepage hero carousel.</summary>
public class HeroSlide : BaseEntity
{
    /// <summary>Slide heading text.</summary>
    public string Title { get; set; }

    /// <summary>Slide sub-text / tagline.</summary>
    public string Description { get; set; }

    /// <summary>Absolute URL of the background image.</summary>
    public string ImageUrl { get; set; }

    /// <summary>Display order; lower values appear first.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this slide is visible on the public site.</summary>
    public bool IsActive { get; set; } = true;
}
