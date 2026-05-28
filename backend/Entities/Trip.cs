namespace backend.Entities;

/// <summary>
/// Represents a single travel trip to a specific province.
/// A province may have many trips, each with its own title, date, and optional story.
/// </summary>
public class Trip : BaseEntity
{
    /// <summary>Foreign key to the <see cref="Province"/> this trip belongs to.</summary>
    public string ProvinceId { get; set; }

    /// <summary>Navigation property for the parent province.</summary>
    public Province Province { get; set; }

    /// <summary>Short descriptive title of the trip.</summary>
    public string Title { get; set; }

    /// <summary>Date the trip took place.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Rich-text HTML story written in the Quill editor. <see langword="null"/> when no story has been added.</summary>
    public string? Story { get; set; }

    /// <summary>UTC timestamp when the trip was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>UTC timestamp of the last modification.</summary>
    public DateTimeOffset UpdatedDate { get; set; }

    /// <summary>Photos attached to this trip, ordered by <see cref="TripPhoto.Order"/>.</summary>
    public ICollection<TripPhoto> TripPhotos { get; set; } = [];
}
