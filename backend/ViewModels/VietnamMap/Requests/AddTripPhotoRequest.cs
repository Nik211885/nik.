using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.VietnamMap.Requests;

/// <summary>Payload for attaching a new photo to a trip.</summary>
public class AddTripPhotoRequest
{
    /// <summary>ID of the trip this photo belongs to.</summary>
    public string TripId { get; set; }

    /// <summary>Cloudinary secure URL.</summary>
    public string Url { get; set; }

    /// <summary>Optional caption.</summary>
    public string? Caption { get; set; }

    /// <summary>Display order within the trip.</summary>
    public int Order { get; set; }
}

/// <summary>Validates <see cref="AddTripPhotoRequest"/>.</summary>
public class AddTripPhotoRequestValidator : AbstractValidator<AddTripPhotoRequest>
{
    /// <inheritdoc/>
    public AddTripPhotoRequestValidator()
    {
        RuleFor(x => x.TripId).NotEmpty();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Caption).MaximumLength(255).When(x => x.Caption != null);
    }
}

/// <summary>Mapper extension for <see cref="AddTripPhotoRequest"/>.</summary>
public static class AddTripPhotoRequestExtensions
{
    extension(AddTripPhotoRequest request)
    {
        /// <summary>Maps the request to a new <see cref="TripPhoto"/> entity.</summary>
        /// <returns>An unsaved <see cref="TripPhoto"/> with UTC timestamps set.</returns>
        public TripPhoto ToTripPhoto() => new TripPhoto
        {
            TripId      = request.TripId,
            Url         = request.Url,
            Caption     = string.IsNullOrWhiteSpace(request.Caption) ? null : request.Caption.Trim(),
            Order       = request.Order,
            CreatedDate = DateTimeOffset.UtcNow,
        };
    }
}
