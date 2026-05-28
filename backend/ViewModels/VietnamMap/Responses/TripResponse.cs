using backend.Entities;

namespace backend.ViewModels.VietnamMap.Responses;

/// <summary>Read-only projection of a <see cref="Trip"/>.</summary>
public class TripResponse
{
    /// <summary>Trip identifier.</summary>
    public string Id { get; set; }

    /// <summary>Foreign key to the parent province.</summary>
    public string ProvinceId { get; set; }

    /// <summary>Short descriptive title of the trip.</summary>
    public string Title { get; set; }

    /// <summary>Date the trip took place.</summary>
    public DateOnly Date { get; set; }

    /// <summary>Rich-text HTML story. <see langword="null"/> when no story has been added.</summary>
    public string? Story { get; set; }

    /// <summary>UTC timestamp when the trip was created.</summary>
    public DateTimeOffset CreatedDate { get; set; }

    /// <summary>Photos for this trip ordered by <see cref="TripPhoto.Order"/>.</summary>
    public List<TripPhotoResponse> Photos { get; set; } = [];
}

/// <summary>Mapping extensions for <see cref="TripResponse"/>.</summary>
public static class TripResponseExtensions
{
    extension(Trip trip)
    {
        /// <summary>Projects the trip entity to a <see cref="TripResponse"/>.</summary>
        /// <returns>A new <see cref="TripResponse"/> instance.</returns>
        public TripResponse ToTripResponse()
        {
            return new TripResponse
            {
                Id          = trip.Id,
                ProvinceId  = trip.ProvinceId,
                Title       = trip.Title,
                Date        = trip.Date,
                Story       = trip.Story,
                CreatedDate = trip.CreatedDate,
            };
        }
    }

    extension(IQueryable<Trip> trips)
    {
        /// <summary>Projects a queryable sequence of <see cref="Trip"/> entities to <see cref="TripResponse"/>, including photos.</summary>
        /// <returns>An <see cref="IQueryable{TripResponse}"/> projection.</returns>
        public IQueryable<TripResponse> ToTripResponses()
        {
            return trips.Select(t => new TripResponse
            {
                Id          = t.Id,
                ProvinceId  = t.ProvinceId,
                Title       = t.Title,
                Date        = t.Date,
                Story       = t.Story,
                CreatedDate = t.CreatedDate,
                Photos      = t.TripPhotos.OrderBy(p => p.Order).Select(p => new TripPhotoResponse
                {
                    Id      = p.Id,
                    TripId  = p.TripId,
                    Url     = p.Url,
                    Caption = p.Caption,
                    Order   = p.Order,
                }).ToList(),
            });
        }
    }
}
