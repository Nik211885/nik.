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

    /// <summary>Total number of reactions from unique devices.</summary>
    public int ReactionCount { get; set; }
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

/// <summary>Returned after toggling a reaction on a wall message.</summary>
public class ReactWallMessageResponse
{
    /// <summary>Updated reaction count after the toggle.</summary>
    public int ReactionCount { get; set; }

    /// <summary><see langword="true"/> if the device just added a reaction; <see langword="false"/> if it was removed.</summary>
    public bool Reacted { get; set; }
}
