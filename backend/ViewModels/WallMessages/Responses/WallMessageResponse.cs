namespace backend.ViewModels.WallMessages.Responses;

/// <summary>Public-facing wall message response (approved messages only).</summary>
public class WallMessageResponse
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; set; }

    /// <summary>Display name of the visitor.</summary>
    public string Name { get; set; }

    /// <summary>Message body.</summary>
    public string Message { get; set; }

    /// <summary>Moderation status — lets the submitter know their message is pending.</summary>
    public string Status { get; set; }

    /// <summary>Optional source or author attribution.</summary>
    public string? Source { get; set; }

    /// <summary>UTC timestamp of submission.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}

/// <summary>Admin-facing wall message response with moderation metadata.</summary>
public class AdminWallMessageResponse
{
    /// <summary>Unique identifier.</summary>
    public string Id { get; set; }

    /// <summary>Display name of the visitor.</summary>
    public string Name { get; set; }

    /// <summary>Message body.</summary>
    public string Message { get; set; }

    /// <summary>Moderation status string.</summary>
    public string Status { get; set; }

    /// <summary>Optional source or author attribution.</summary>
    public string? Source { get; set; }

    /// <summary>Submitter IP address.</summary>
    public string? IpAddress { get; set; }

    /// <summary>UTC timestamp of submission.</summary>
    public DateTimeOffset CreatedDate { get; set; }
}
