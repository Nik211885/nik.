namespace backend.Entities;

/// <summary>
/// Represents one of Vietnam's 63 administrative provinces or municipalities.
/// Instances are seeded on startup and never created via API.
/// </summary>
public class Province : BaseEntity
{
    /// <summary>Full Vietnamese name, e.g. <c>Hà Nội</c>. Must match the GeoJSON feature name used by the Leaflet map.</summary>
    public string Name { get; set; }

    /// <summary>URL-safe slug identifier, e.g. <c>ha-noi</c>. Stable key used in API URLs and GeoJSON matching.</summary>
    public string Code { get; set; }

    /// <summary>Trips recorded for this province.</summary>
    public ICollection<Trip> Trips { get; set; }
}
