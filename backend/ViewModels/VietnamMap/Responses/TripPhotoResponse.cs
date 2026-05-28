using backend.Entities;

namespace backend.ViewModels.VietnamMap.Responses;

/// <summary>Read-only projection of a <see cref="TripPhoto"/>.</summary>
public class TripPhotoResponse
{
    /// <summary>Photo identifier.</summary>
    public string Id { get; set; }

    /// <summary>Foreign key to the parent trip.</summary>
    public string TripId { get; set; }

    /// <summary>Cloudinary secure URL.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption.</summary>
    public string? Caption { get; set; }

    /// <summary>Display order (ascending).</summary>
    public int Order { get; set; }
}

/// <summary>Mapping extensions for <see cref="TripPhotoResponse"/>.</summary>
public static class TripPhotoResponseExtensions
{
    extension(TripPhoto photo)
    {
        /// <summary>Projects the photo entity to a <see cref="TripPhotoResponse"/>.</summary>
        /// <returns>A new <see cref="TripPhotoResponse"/> instance.</returns>
        public TripPhotoResponse ToTripPhotoResponse() => new TripPhotoResponse
        {
            Id      = photo.Id,
            TripId  = photo.TripId,
            Url     = photo.Url,
            Caption = photo.Caption,
            Order   = photo.Order,
        };
    }
}
