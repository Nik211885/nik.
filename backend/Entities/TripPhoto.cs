namespace backend.Entities;

/// <summary>
/// A photo attached to a <see cref="Trip"/>, uploaded to Cloudinary.
/// </summary>
public class TripPhoto : BaseEntity
{
    /// <summary>Foreign key to the parent <see cref="Trip"/>.</summary>
    public string TripId { get; set; }

    /// <summary>Navigation property for the parent trip.</summary>
    public Trip Trip { get; set; }

    /// <summary>Cloudinary secure URL of the photo.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption displayed beneath the photo.</summary>
    public string? Caption { get; set; }

    /// <summary>Display order within the trip (ascending).</summary>
    public int Order { get; set; }

    /// <summary>UTC timestamp when the photo was attached.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}
