using backend.Entities;
using FluentValidation;

namespace backend.ViewModels.VietnamMap.Requests;

/// <summary>Payload for creating a new <see cref="Trip"/>.</summary>
public class CreateTripRequest
{
    /// <summary>ID of the province this trip belongs to.</summary>
    public string ProvinceId { get; set; }

    /// <summary>Short descriptive title of the trip.</summary>
    public string Title { get; set; }

    /// <summary>Date the trip took place (ISO 8601 date string, e.g. <c>2024-03-15</c>).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Rich-text HTML story from the Quill editor. Optional.</summary>
    public string? Story { get; set; }
}

/// <summary>Validates <see cref="CreateTripRequest"/> inputs.</summary>
public class CreateTripRequestValidator : AbstractValidator<CreateTripRequest>
{
    /// <inheritdoc/>
    public CreateTripRequestValidator()
    {
        RuleFor(x => x.ProvinceId)
            .NotEmpty()
            .WithMessage(ApplicationMessage.TripProvinceRequired);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(ApplicationMessage.TitleIsRequired)
            .MaximumLength(255)
            .WithMessage(ApplicationMessage.TitleIsRequired);
    }
}

/// <summary>Mapping extensions for <see cref="CreateTripRequest"/>.</summary>
public static class CreateTripRequestExtensions
{
    extension(CreateTripRequest request)
    {
        /// <summary>Maps the request to a new <see cref="Trip"/> entity with timestamps set to UTC now.</summary>
        /// <returns>A new unsaved <see cref="Trip"/> instance.</returns>
        public Trip ToTrip()
        {
            var now = DateTimeOffset.UtcNow;
            return new Trip
            {
                ProvinceId  = request.ProvinceId,
                Title       = request.Title,
                Date        = request.Date,
                Story       = request.Story,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }
    }
}
