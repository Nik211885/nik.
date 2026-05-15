namespace backend.ViewModels.HeroSlides.Responses;

/// <summary>Response DTO for a single hero slide.</summary>
public class HeroSlideResponse
{
    /// <summary>Slide ID.</summary>
    public string Id { get; set; }

    /// <summary>Slide heading text.</summary>
    public string Title { get; set; }

    /// <summary>Slide sub-text.</summary>
    public string Description { get; set; }

    /// <summary>Background image URL.</summary>
    public string ImageUrl { get; set; }

    /// <summary>Display order index.</summary>
    public int OrderIndex { get; set; }

    /// <summary>Whether this slide is visible on the public site.</summary>
    public bool IsActive { get; set; }
}
