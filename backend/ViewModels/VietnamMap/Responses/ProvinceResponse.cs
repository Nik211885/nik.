using backend.Entities;

namespace backend.ViewModels.VietnamMap.Responses;

/// <summary>Read-only projection of a <see cref="Province"/> with its trip count.</summary>
public class ProvinceResponse
{
    /// <summary>Province identifier.</summary>
    public string Id { get; set; }

    /// <summary>Full Vietnamese province name.</summary>
    public string Name { get; set; }

    /// <summary>URL-safe slug identifier.</summary>
    public string Code { get; set; }

    /// <summary>Number of trips recorded for this province.</summary>
    public int TripCount { get; set; }
}

/// <summary>Mapping extensions for <see cref="ProvinceResponse"/>.</summary>
public static class ProvinceResponseExtensions
{
    extension(Province province)
    {
        /// <summary>Projects the province to a <see cref="ProvinceResponse"/> with zero trip count.</summary>
        /// <returns>A new <see cref="ProvinceResponse"/> instance.</returns>
        public ProvinceResponse ToProvinceResponse()
        {
            return new ProvinceResponse
            {
                Id        = province.Id,
                Name      = province.Name,
                Code      = province.Code,
                TripCount = province.Trips?.Count ?? 0,
            };
        }
    }
}
